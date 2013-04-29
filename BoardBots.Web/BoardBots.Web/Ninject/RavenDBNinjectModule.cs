using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Document;

namespace BoardBots.Web.Ninject
{
    public class RavenDBNinjectModule : NinjectModule
    {
        public override void Load()
        {

            Bind<IDocumentStore>().ToMethod(CreateDocumentStore).InSingletonScope();
            Bind<IDocumentSession>().ToMethod(OpenDocumentSession).InRequestScope();
        }
        private static IDocumentStore CreateDocumentStore(IContext context)
        {
            DocumentStore documentStore = new DocumentStore
            {
                ConnectionStringName = "RavenHQ"
            };
            documentStore.Initialize();
            return documentStore;
        }

        private IDocumentSession OpenDocumentSession(IContext context)
        {
            IDocumentStore documentStore = (IDocumentStore)Kernel.GetService(typeof(IDocumentStore));
            return documentStore.OpenSession();
        }
    }
}