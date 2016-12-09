using Autofac;
using DependencyInjection.Console.CharacterWriters;
using DependencyInjection.Console.SquarePainters;

namespace DependencyInjection.Console
{
    public class InjectionModule : Module
    {
        public bool UseColors { get; set; }
        public string Pattern { get; set; }
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c => new AsciiWriter()).Keyed<ICharacterWriter>(false);
            containerBuilder.Register(c => new ColorWriter(c.ResolveKeyed<ICharacterWriter>(false))).Keyed<ICharacterWriter>(true);
            containerBuilder.Register(c => new PatternWriter(c.ResolveKeyed<ICharacterWriter>(UseColors))).AsSelf();

            containerBuilder.Register(c => new CircleSquarePainter()).Keyed<ISquarePainter>("circle");
            containerBuilder.Register(c => new OddEvenSquarePainter()).Keyed<ISquarePainter>("oddeven");
            containerBuilder.Register(c => new WhiteSquarePainter()).Keyed<ISquarePainter>("white");

            containerBuilder.Register(c => new PatternGenerator(c.ResolveKeyed<ISquarePainter>(Pattern))).AsSelf();

            containerBuilder.Register(
                c =>
                    new PatternApp(c.Resolve<PatternWriter>(), c.Resolve<PatternGenerator>()))
                .AsSelf();
        }
    }
}