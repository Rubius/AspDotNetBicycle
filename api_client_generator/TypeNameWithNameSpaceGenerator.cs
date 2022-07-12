using System.Reflection;
using NJsonSchema;

namespace SwaggerGen
{
    public class TypeNameWithNameSpaceGenerator : DefaultTypeNameGenerator
    {
        private readonly Dictionary<string, Type> _types;

        public TypeNameWithNameSpaceGenerator()
        {
            _types = new Dictionary<string, Type>();

            IEnumerable<AssemblyName> assemblies = AssembliesHelper
                .GetWebAppAssembly()
                .GetReferencedAssemblies();

            var application = assemblies
                .FirstOrDefault(x => x.Name?.Contains("Application") ?? false);
            if (application != null)
            {
                assemblies = assemblies.Union(Assembly.Load(application).GetReferencedAssemblies());
            }

            var allTypes = assemblies.Select(Assembly.Load)
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes);

            foreach (var type in allTypes)
            {
                if (!_types.ContainsKey(type.Name))
                {
                    _types[type.Name] = type;
                }
            }
        }

        public override string Generate(
            JsonSchema schema,
            string typeNameHint,
            IEnumerable<string> reservedTypeNames)
        {
            if (_types.TryGetValue(typeNameHint, out var type))
            {
                return type.FullName!;
            }

            throw new ArgumentOutOfRangeException(nameof(typeNameHint), typeNameHint, "Unknown type");
        }
    }
}