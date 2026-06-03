import * as http from "node:http";

const MAX_BODY_SIZE = 1024 * 1024; // 1 MB

export interface RouteRequest {
  readonly method: string;
  readonly url: URL;
  readonly params: Record<string, string>;
  readonly raw: http.IncomingMessage;
}

export interface RouteResponse {
  readonly status: number;
  readonly headers: Record<string, string>;
  readonly body: string;
}

type RouteHandler = (req: RouteRequest) => Promise<RouteResponse> | RouteResponse;

interface Route {
  method: string;
  pattern: RegExp;
  paramNames: string[];
  handler: RouteHandler;
}

export function json<T>(status: number, body: T): RouteResponse {
  return {
    status,
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  };
}

export function text(status: number, body: string): RouteResponse {
  return {
    status,
    headers: { "Content-Type": "text/plain" },
    body,
  };
}

export async function jsonBody<T>(req: RouteRequest): Promise<T> {
  const raw = await readBody(req.raw);
  return JSON.parse(raw) as T;
}

export class Router {
  readonly #routes: Route[] = [];

  get(path: string, handler: RouteHandler): void {
    this.#addRoute("GET", path, handler);
  }

  post(path: string, handler: RouteHandler): void {
    this.#addRoute("POST", path, handler);
  }

  #addRoute(method: string, path: string, handler: RouteHandler): void {
    const paramNames: string[] = [];
    const patternStr = path.replace(/:([^/]+)/g, (_match, name) => {
      paramNames.push(name);
      return "([^/]+)";
    });
    const pattern = new RegExp(`^${patternStr}$`);
    this.#routes.push({ method, pattern, paramNames, handler });
  }

  async handle(req: http.IncomingMessage, res: http.ServerResponse): Promise<void> {
    const url = new URL(req.url ?? "/", `http://${req.headers.host}`);
    const method = req.method ?? "GET";

    for (const route of this.#routes) {
      if (route.method !== method) continue;
      const match = url.pathname.match(route.pattern);
      if (!match) continue;

      const params: Record<string, string> = {};
      route.paramNames.forEach((name, i) => {
        params[name] = decodeURIComponent(match[i + 1]);
      });

      const routeReq: RouteRequest = { method, url, params, raw: req };
      const routeRes = await route.handler(routeReq);

      res.writeHead(routeRes.status, routeRes.headers);
      res.end(routeRes.body);
      return;
    }

    res.writeHead(404, { "Content-Type": "text/plain" });
    res.end("Not Found");
  }
}

function readBody(req: http.IncomingMessage): Promise<string> {
  return new Promise((resolve, reject) => {
    const chunks: Buffer[] = [];
    let size = 0;
    req.on("data", (chunk: Buffer) => {
      size += chunk.length;
      if (size > MAX_BODY_SIZE) {
        req.destroy();
        reject(new Error("Request body too large"));
        return;
      }
      chunks.push(chunk);
    });
    req.on("end", () => resolve(Buffer.concat(chunks).toString()));
    req.on("error", reject);
  });
}
