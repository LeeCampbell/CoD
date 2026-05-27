import { describe, it } from "node:test";
import assert from "node:assert/strict";
import { BankAccount, InvalidBankAccountError } from "@cod/domain";

describe("BankAccount", () => {
  it("creates with valid BSB and account number", () => {
    const account = BankAccount.create("066-000", "12345678");
    assert.strictEqual(account.bsb, "066-000");
    assert.strictEqual(account.accountNumber, "12345678");
  });

  it("normalises BSB without hyphen", () => {
    const account = BankAccount.create("066000", "12345678");
    assert.strictEqual(account.bsb, "066-000");
  });

  it("accepts minimum length account number (3 digits)", () => {
    const account = BankAccount.create("066-000", "123");
    assert.strictEqual(account.accountNumber, "123");
  });

  it("accepts maximum length account number (12 digits)", () => {
    const account = BankAccount.create("066-000", "123456789012");
    assert.strictEqual(account.accountNumber, "123456789012");
  });

  // Invalid BSB formats
  it("throws when BSB is empty", () => {
    assert.throws(() => BankAccount.create("", "12345678"), {
      message: "BSB is required",
    });
  });

  it("throws when BSB is whitespace", () => {
    assert.throws(() => BankAccount.create("   ", "12345678"), {
      message: "BSB is required",
    });
  });

  for (const invalidBsb of [
    "12345",       // 5 digits
    "Abc-123",     // letters in bank
    "Abc123",      // letters no hyphen
    "00600",       // 5 digits
    "066-00",      // 3+2 digits
    "066-0000",    // 3+4 digits
    "0066-000",    // 4+3 digits
    "06-6000",     // 2+4 digits
  ]) {
    it(`throws for invalid BSB format: "${invalidBsb}"`, () => {
      assert.throws(
        () => BankAccount.create(invalidBsb, "12345678"),
        (e: unknown) => {
          assert.ok(e instanceof InvalidBankAccountError);
          assert.strictEqual(e.message, "BSB is not valid");
          assert.strictEqual(e.field, "bsb");
          return true;
        },
      );
    });
  }

  // Invalid account number formats
  it("throws when account number is empty", () => {
    assert.throws(() => BankAccount.create("066-000", ""), {
      message: "Account number is required",
    });
  });

  for (const invalidAccount of [
    "1",             // 1 digit (under min 3)
    "12",            // 2 digits (under min 3)
    "1234567890123", // 13 digits (over max 12)
    "123a",          // non-numeric
    "a123",          // non-numeric
    " 123",          // leading space
  ]) {
    it(`throws for invalid account number: "${invalidAccount}"`, () => {
      assert.throws(
        () => BankAccount.create("066-000", invalidAccount),
        (e: unknown) => {
          assert.ok(e instanceof InvalidBankAccountError);
          assert.strictEqual(e.field, "accountNumber");
          return true;
        },
      );
    });
  }
});
