using ResourceGenerator;
using ResourceGenerator.Interfaces;

IGenerator generator = new Generator();

switch (args[0])
{
    case "java":
        {
            generator.Generate(args[1], args[2], IGenerator.ClientTypeEnum.Java);
            break;
        }
    case "dotnet":
        {
            generator.Generate(args[1], args[2], IGenerator.ClientTypeEnum.Net);
            break;
        }
    default:
        {
            throw new NotSupportedException("Unknown client type - please use either java or dotnet as first argument");
        }
}
