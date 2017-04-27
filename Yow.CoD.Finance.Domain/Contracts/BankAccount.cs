using System;
using System.Text.RegularExpressions;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class BankAccount
    {
        private static readonly Regex BsbRegex = new Regex("^(?<bank>\\d{3})-?(?<branch>\\d{3})$", RegexOptions.Compiled);
        private static readonly Regex AccountNumberRegex = new Regex("^\\d{3,12}$", RegexOptions.Compiled);
        public BankAccount(string bsb, string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(bsb))
                throw new ArgumentException("BSB is required", nameof(bsb));
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required", nameof(accountNumber));

            var bsbMatch = BsbRegex.Match(bsb);
            if (!bsbMatch.Success)
                throw new ArgumentException("BSB is not valid", nameof(bsb));

            if (!AccountNumberRegex.IsMatch(accountNumber))
                throw new ArgumentException("Account number is not valid", nameof(accountNumber));

            Bsb = $"{bsbMatch.Groups["bank"].Value}-{bsbMatch.Groups["branch"].Value}";
            AccountNumber = accountNumber;
        }

        public string Bsb { get; }
        public string AccountNumber { get; }
    }
}