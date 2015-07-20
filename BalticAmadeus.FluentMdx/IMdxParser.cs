namespace BalticAmadeus.FluentMdx
{
    public interface IMdxParser
    {
        MdxQuery ParseQuery(string source);
    }
}