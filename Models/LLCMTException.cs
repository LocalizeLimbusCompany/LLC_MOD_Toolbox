namespace LLC_MOD_Toolbox.Models
{
    public class HashException : Exception
    {
    }
    public class MirrorChyanException : Exception
    {
        private int errorID;
        public MirrorChyanException(int errorID)
        {
            this.errorID = errorID;
        }
        public override string Message
        {
            get {
                switch (errorID)
                {
                    case 0:
                        return "不是哥们，这不会有问题啊，这条提示绝对不可能出现，要是出现了我穿女装";
                    case 7001:
                        return "您的 Mirror 酱 CDK 已经过期，请前往 Mirror 酱官网购买。";
                    case 7002:
                        return "您的 Mirror 酱 CDK 无效，请确保您输入了正确的 CDK。";
                    case 7003:
                        return "您的 Mirror 酱 CDK 已经达到使用上限，请隔天再试。";
                    case 7004:
                        return "您的 Mirror 酱 CDK 可能为特殊 CDK，无法用于工具箱，请前往 Mirror 酱官网购买。";
                    case 7005:
                        return "您的 Mirror 酱 CDK 被冻结，请前往 Mirror 酱售后群询问。";
                    default:
                        return "在解析您的 Mirror 酱请求时发生了未知错误。";
                }
            }
        }
    }
}
