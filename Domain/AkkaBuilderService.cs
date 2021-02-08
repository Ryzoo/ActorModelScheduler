using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DependencyInjection;
using Domain.Actors;
using Domain.Messages.Actions;
using Infrastructure.Akka;
using Microsoft.Extensions.Hosting;

namespace Domain
{
    public class AkkaBuilderService : IAkkaBuilderService, IHostedService
    {
        private readonly IServiceProvider _provider;
        private ActorSystem _actorSystem;
        private IActorRef _mailControllerActor;

        public AkkaBuilderService(IServiceProvider sp)
        {
            _provider = sp;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var di = ServiceProviderSetup.Create(_provider);
            _actorSystem = ActorSystem.Create("ActorModelScheduler", BootstrapSetup.Create().And(di));
            
            InitializeRootActors();
            InitializeScheduler();
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void InitializeRootActors()
        {
            var mailControllerProps = ServiceProvider.For(_actorSystem).Props<MailControllerActor>();
            _mailControllerActor = _actorSystem.ActorOf(mailControllerProps, "MailControllerActor");
        }

        private void InitializeScheduler()
        {
            _actorSystem
                .Scheduler
                .ScheduleTellRepeatedly(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5),
                    _mailControllerActor, new StandUpMessage(), ActorRefs.NoSender); 
        }
    }
}
    