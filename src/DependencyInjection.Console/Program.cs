using Autofac;
using DependencyInjection.Console.CharacterWriters;
using DependencyInjection.Console.SquarePainters;
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
            containerBuilder.Register(c => new AsciiWriter()).Keyed<ICharacterWriter>(false);
            containerBuilder.Register(c => new ColorWriter(c.ResolveKeyed<ICharacterWriter>(false))).Keyed<ICharacterWriter>(true);
            containerBuilder.Register(c => new PatternWriter(c.ResolveKeyed<ICharacterWriter>(useColors))).AsSelf();

            containerBuilder.Register(c => new CircleSquarePainter()).Keyed<ISquarePainter>("circle");
            containerBuilder.Register(c => new OddEvenSquarePainter()).Keyed<ISquarePainter>("oddeven");
            containerBuilder.Register(c => new WhiteSquarePainter()).Keyed<ISquarePainter>("white");

            containerBuilder.Register(c => new PatternGenerator(c.ResolveKeyed<ISquarePainter>(pattern))).AsSelf();

            containerBuilder.Register(
                c =>
                    new PatternApp(c.Resolve<PatternWriter>(), c.Resolve<PatternGenerator>()))
                .AsSelf();

            m_Container = containerBuilder.Build();
        }
    }
}
