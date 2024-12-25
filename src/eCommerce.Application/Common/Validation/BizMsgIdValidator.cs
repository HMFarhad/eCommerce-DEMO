using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Serialization;
using static eCommerce.Application.Common.EnumCollections;

namespace eCommerce.Application.Common.Validation;

public class BizMsgIdValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Business Message Identification is required.");
        }
        string bizMsgId = value.ToString();
        if (bizMsgId.Length < 12 || bizMsgId.Length > 35)
        {
            return new ValidationResult("Business Message Identification length must be between 12 to 35 characters");
        }

        /*
            FORMAT: YYYYMMDDBBBBBBBBXXXOCCSSSSSSSS where

            YYYYMMDD – Current Date
            BBBBBBBB – BIC Code
            XXX – Transaction Code. Set to 510 for Account Inquiry. Refer to Transaction Code
            O – Originator. Refer to Originator
            CC – Channel Code. Refer to Channel Code
            SSSSSSSS – Sequence Number
         */

        string date = bizMsgId.Substring(0, 8);
        string bicCode = bizMsgId.Substring(8, 8);
        string transactionCode = bizMsgId.Substring(16, 3);
        string originator = bizMsgId.Substring(19, 1);
        string channelCode = bizMsgId.Substring(20, 2);
        string sequenceNumber = bizMsgId.Substring(22);

        if (date.Length != 8 || bicCode.Length != 8 || transactionCode.Length != 3 || originator.Length != 1 || channelCode.Length != 2 || sequenceNumber.Length < 1)
        {
            return new ValidationResult("Invalid Business Message Identification format.");
        }

        return ValidationResult.Success;
    }
}
