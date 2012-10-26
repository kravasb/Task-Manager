using System;
using System.Collections.Generic;
using System.ServiceModel;
using EntitiesLibrary;
using EntitiesLibrary.CommandArguments;
using FluentAssertions;
using NSubstitute;
using Specifications.ClientSpecifications;
using Specifications.ServiceSpecifications;
using TaskManagerServiceLibrary;
using TaskManagerServiceLibrary.Repositories;
using Xunit;

namespace ConnectToWcf
{
    public class ExchangeClient : IClient
    {
        private readonly NetTcpBinding binding;
        private readonly string serviceAddress;

        public ExchangeClient(string address)
        {
            serviceAddress = address;
            binding = new NetTcpBinding();
        }

        public int AddTask(AddTaskArgs task)
        {
            return GetDataFromServer(t => t.AddTask(task));
        }

        public List<ClientPackage> GetTasks(IClientSpecification data)
        {
            return GetDataFromServer(s => s.GetTasks(data));
        }

        public void UpdateChanges(ICommandArguments args)
        {
            UpdateDataOnServer(s=>s.UpdateChanges(args));
        }

        private void UpdateDataOnServer(Action<ITaskManagerService> action)
        {
            GetDataFromServer<object>(s =>
                                          {
                                              action(s);
                                              return null;
                                          });
        }

        private T GetDataFromServer<T>(Func<ITaskManagerService, T> func)
        {
            var client = new ChannelFactory<ITaskManagerService>(binding, serviceAddress);
            client.Open();
            try
            {
                return func(client.CreateChannel());
            }
            catch (FaultException<ExceptionDetail> e)
            {
                throw new TaskNotFoundException(e.Detail.Message);
            }
            finally
            {
                CloseClient(client);
            }
        }

        private static void CloseClient(ChannelFactory<ITaskManagerService> client)
        {
            try
            {
                client.Close();
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }
    }

    public class ExchangeClientTests
    {
        private const string address = "net.tcp://localhost:44440";

        private readonly IRepository repo = Substitute.For<IRepository>();
        private readonly IClient client = new ExchangeClient(address);
        private readonly ITaskManagerService service;
        private readonly ServiceHost host;

        public ExchangeClientTests()
        {
            service = new TaskManagerService(repo);
            host = new ServiceHost(service, new Uri(address));
            host.Open();
        }

        [Fact]
        public void should_add_task_to_service()
        {
            const string myname = "myName";
            var args = new AddTaskArgs { Name = myname };
            repo.AddTask(Arg.Is<AddTaskArgs>(a => a.Name == myname)).Returns(1);
            
            var result = client.AddTask(args);
            host.Close();

            result.Should().Be(1);
        }

        [Fact(Skip = "")]
        public void should_get_tasks_from_server()
        {
            var expected = new List<ClientPackage> {new ClientPackage {Id = 1}};
            var cSpec = Substitute.For<IClientSpecification>();
            //cSpec.Data.Returns(1);
            var qSpec = Substitute.For<IServiceSpecification>();

            repo.GetTasks(Arg.Is<IServiceSpecification>(s => s.IsSatisfied(new ServiceTask{Id = 1}))).Returns(expected);

            var result = client.GetTasks(cSpec);

            foreach (var task in result)
            {
                Console.WriteLine(task);
            }

            host.Close();
        }
    }
}