using System;
using Microsoft.CodeAnalysis;

namespace DeviceManagementPropertiesGenerator
{
    [Generator]
    public class DevicePropertyKeyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}