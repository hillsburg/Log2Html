using Autofac;

namespace Log2Html.Utils
{
    internal class ViewModelLocator
    {
        private readonly IContainer _container;

        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            _container = builder.Build();
        }
    }
}
