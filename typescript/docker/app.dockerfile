FROM node:22-alpine AS builder
WORKDIR /app
COPY package.json package-lock.json ./
COPY packages/domain/package.json packages/domain/
COPY packages/pg-data-adapter/package.json packages/pg-data-adapter/
COPY packages/web-comms-adapter/package.json packages/web-comms-adapter/
COPY packages/app/package.json packages/app/
RUN npm ci
COPY tsconfig.json tsconfig.build.json ./
COPY packages/domain/ packages/domain/
COPY packages/pg-data-adapter/ packages/pg-data-adapter/
COPY packages/web-comms-adapter/ packages/web-comms-adapter/
COPY packages/app/ packages/app/
RUN npx tsc --build tsconfig.build.json

FROM node:22-alpine AS runtime
WORKDIR /app
COPY package.json package-lock.json ./
COPY packages/domain/package.json packages/domain/
COPY packages/pg-data-adapter/package.json packages/pg-data-adapter/
COPY packages/web-comms-adapter/package.json packages/web-comms-adapter/
COPY packages/app/package.json packages/app/
RUN npm ci --omit=dev
COPY --from=builder /app/packages/domain/dist packages/domain/dist
COPY --from=builder /app/packages/pg-data-adapter/dist packages/pg-data-adapter/dist
COPY --from=builder /app/packages/web-comms-adapter/dist packages/web-comms-adapter/dist
COPY --from=builder /app/packages/app/dist packages/app/dist

EXPOSE 4567
CMD ["node", "packages/app/dist/main.js"]
