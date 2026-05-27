import { describe, it } from "node:test";
import assert from "node:assert/strict";
import { CustomerContact, InvalidCustomerContactError } from "@cod/domain";

describe("CustomerContact", () => {
  it("creates with valid data", () => {
    const contact = CustomerContact.create(
      "Jane Doe",
      "0412341234",
      "0856785678",
      "10 St Georges Terrace, Perth, WA 6000",
    );
    assert.strictEqual(contact.name, "Jane Doe");
    assert.strictEqual(contact.preferredPhoneNumber, "0412341234");
    assert.strictEqual(contact.alternatePhoneNumber, "0856785678");
    assert.strictEqual(
      contact.postalAddress,
      "10 St Georges Terrace, Perth, WA 6000",
    );
  });

  // Various valid phone prefixes
  for (const phone of ["0412341234", "0212341234", "0812341234", "0444444444"]) {
    it(`accepts valid phone number: ${phone}`, () => {
      const contact = CustomerContact.create("Jane", phone, undefined, "addr");
      assert.strictEqual(contact.preferredPhoneNumber, phone);
    });
  }

  it("creates with undefined alternate phone", () => {
    const contact = CustomerContact.create(
      "Jane Doe",
      "0412341234",
      undefined,
      "10 St Georges Terrace",
    );
    assert.strictEqual(contact.alternatePhoneNumber, undefined);
  });

  // Name validation
  it("throws when name is empty", () => {
    assert.throws(
      () => CustomerContact.create("", "0412341234", undefined, "address"),
      (e: unknown) => {
        assert.ok(e instanceof InvalidCustomerContactError);
        assert.strictEqual(e.field, "name");
        return true;
      },
    );
  });

  it("throws when name is whitespace", () => {
    assert.throws(
      () => CustomerContact.create("   ", "0412341234", undefined, "address"),
      { message: "Name is required" },
    );
  });

  // Preferred phone validation
  it("throws when preferred phone is empty", () => {
    assert.throws(
      () => CustomerContact.create("Jane", "", undefined, "address"),
      { message: "Preferred phone number is required" },
    );
  });

  for (const invalidPhone of [
    "0",               // single digit
    "1234",            // 4 digits, no leading 0
    "12345",           // 5 digits
    "04123456",        // 8 digits
    "041234567",       // 9 digits
    "04123456789",     // 11 digits
    "04123456789012",  // 14 digits
  ]) {
    it(`throws for invalid preferred phone: "${invalidPhone}"`, () => {
      assert.throws(
        () => CustomerContact.create("Jane", invalidPhone, undefined, "addr"),
        (e: unknown) => {
          assert.ok(e instanceof InvalidCustomerContactError);
          assert.strictEqual(e.field, "preferredPhoneNumber");
          return true;
        },
      );
    });
  }

  // Alternate phone validation
  it("throws when alternate phone is invalid", () => {
    assert.throws(
      () => CustomerContact.create("Jane", "0412341234", "12345", "address"),
      (e: unknown) => {
        assert.ok(e instanceof InvalidCustomerContactError);
        assert.strictEqual(e.field, "alternatePhoneNumber");
        return true;
      },
    );
  });

  // Postal address validation
  it("throws when postal address is empty", () => {
    assert.throws(
      () => CustomerContact.create("Jane", "0412341234", undefined, ""),
      { message: "Postal address is required" },
    );
  });

  it("throws when postal address is whitespace", () => {
    assert.throws(
      () => CustomerContact.create("Jane", "0412341234", undefined, "   "),
      { message: "Postal address is required" },
    );
  });
});
