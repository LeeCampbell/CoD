package com.leecampbell.cod.domain.contracts;

public final class LoanCustomerContactChangedEvent extends DomainEvent
{
    private final CustomerContact customerContact;

    public LoanCustomerContactChangedEvent(String customerContactName, 
            String customerContactPreferredPhoneNumber,
            String customerContactAlternatePhoneNumber, 
            String customerContactPostalAddress) {
        
        this.customerContact = new CustomerContact(
            customerContactName, 
            customerContactPreferredPhoneNumber,
            customerContactAlternatePhoneNumber, 
            customerContactPostalAddress);
    }

    public CustomerContact customerContact() {
        return customerContact;
    }
}