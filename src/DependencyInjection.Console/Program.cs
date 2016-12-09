using System;
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
                {"p|pattern=", value => pattern = value}
            };
            optionSet.Parse(args);

            BuildContainer();

            var characterWriter = m_Container.ResolveKeyed<ICharacterWriter>(useColors);
            var patternWriter = new PatternWriter(characterWriter);
            var squarePainter = GetSquarePainter(pattern);
            var patternGenerator = new PatternGenerator(squarePainter);

            var app = new PatternApp(patternWriter, patternGenerator);
            app.Run(width, height);
        }

        private static ICharacterWriter GetCharacterWriter(bool useColors)
        {
            var writer = new AsciiWriter();
            return useColors ? (ICharacterWriter) new ColorWriter(writer) : writer;
        }

        private static ISquarePainter GetSquarePainter(string pattern)
        {
            switch (pattern)
            {
                case "circle":
                    return new CircleSquarePainter();
                case "oddeven":
                    return new OddEvenSquarePainter();
                case "white":
                    return new WhiteSquarePainter();
                default:
                    throw new ArgumentException($"Pattern '{pattern}' not found!");
            }
        }

        private static void BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(c => new AsciiWriter()).Keyed<ICharacterWriter>(false);
            containerBuilder.Register(c => new ColorWriter(c.ResolveKeyed<ICharacterWriter>(false))).Keyed<ICharacterWriter>(true);
            m_Container = containerBuilder.Build();
        }
    }
}
