namespace eCommerce.Application.Common;

public static class EnumCollections
{

    public enum demoTxnCodeEnum
    {
        DuitNowAccountInquiry,
        DuitNowCreditTransfer,
        demoSomethingReversal,
        DuitNowTxnInquiry,

        DemoSomethingAcccountInquiry,
        DemoSomethingPaymentPOS,
        DemoSomethingPaymentP2P,
        DemoSomethingTxnInquiry,
    }

    public static readonly Dictionary<demoTxnCodeEnum, string> demoTxnCodeMap = new Dictionary<demoTxnCodeEnum, string>
    {
        { demoTxnCodeEnum.DuitNowAccountInquiry,      "510" },
        { demoTxnCodeEnum.DuitNowCreditTransfer,      "010" },
        { demoTxnCodeEnum.demoSomethingReversal,    "011" },
        { demoTxnCodeEnum.DuitNowTxnInquiry,          "630" },

        { demoTxnCodeEnum.DemoSomethingAcccountInquiry,   "520" },
        { demoTxnCodeEnum.DemoSomethingPaymentPOS,        "030" },
        { demoTxnCodeEnum.DemoSomethingPaymentP2P,        "040" },
        { demoTxnCodeEnum.DemoSomethingTxnInquiry,        "630" }
    };

    public static string GetdemoTxnCode(demoTxnCodeEnum code)
    {
        return demoTxnCodeMap.TryGetValue(code, out var value) ? value : string.Empty;
    }
}
