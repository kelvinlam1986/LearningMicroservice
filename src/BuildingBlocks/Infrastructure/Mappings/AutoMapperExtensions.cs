using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.Mappings
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> experssion)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties ) 
            { 
                if (sourceType.GetProperty(property.Name, flags) == null)
                {
                    experssion.ForMember(property.Name, opt => opt.Ignore());
                }
            }

            return experssion;
        }
    }
}
