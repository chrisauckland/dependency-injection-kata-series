using Autofac;
using NDesk.Options;

namespace DependencyInjection.Console
{
    internal class Program
    {
        private static IContainer m_Container;

        private static void Main(string[] args)
        {
            var useColors = false;
            var width = 25;
            var height = 15;
            var pattern = "circle";

            var optionSet = new OptionSet
            {
                {"c|colors", value => useColors = value != null},
                {"w|width=", value => width = int.Parse(value)},
                {"h|height=", value => height = int.Parse(value)},
                {"p|pattern=", value => pattern = value.ToLower()}
            };
            optionSet.Parse(args);

            BuildContainer(useColors, pattern);

            var app = m_Container.Resolve<PatternApp>();

            app.Run(width, height);
        }

        private static void BuildContainer(bool useColors, string pattern)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new InjectionModule() {UseColors = useColors, Pattern = pattern}) ;
            m_Container = containerBuilder.Build();
        }
    }
}
