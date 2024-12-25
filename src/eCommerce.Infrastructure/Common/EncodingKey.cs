namespace eCommerce.Infrastructure.Common;

#nullable disable
public class EncodingKey
{
    public string Key { get; set; }
    public string IV { get; set; }
    public string Key2 { get; set; }
    public string IVGeneral { get; set; }
    public string Key_DBEncrypt { get; set; }
    public string IV_DBEncrypt { get; set; }
    public string AlgoKey { get; set; }
    public string AlgoIV { get; set; }


}
