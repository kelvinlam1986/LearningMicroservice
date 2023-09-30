using AutoMapper;
using System.Reflection;

namespace Ordering.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var mapFromType = typeof(IMapFrom<>);
            const string mappingMethodName = nameof(IMapFrom<object>.Mapping);
            bool hasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(hasInterface)).ToList();
            var argumentTypes = new Type[] { typeof(Profile) };
            foreach ( var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod(mappingMethodName);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(instance, new object[] { this });
                }
                else
                {
                    var interfaces = type.GetInterfaces().Where(hasInterface).ToList();
                    if (interfaces.Count <= 0)
                    {
                        continue;
                    }

                    foreach ( var iface in interfaces)
                    {
                        var intefaceMethodInfo = iface.GetMethod(mappingMethodName, argumentTypes);
                        intefaceMethodInfo?.Invoke(instance, new object[] { this });
                    }
                }
            }
        }
    }
}
