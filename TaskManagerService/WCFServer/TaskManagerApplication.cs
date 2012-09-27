using System;
using System.ServiceModel;
using EntitiesLibrary;
using Ninject;
using Ninject.Modules;
using TaskManagerHost.DataBaseAccessLayer;
using TaskManagerHost.TaskManager;

namespace TaskManagerHost.WCFServer
{
    static class TaskManagerApplication
    {
        const string Address = "net.tcp://localhost:44444";

        static void Main()
        {
            using (var serviceHost = new ServiceHost(typeof(TaskManagerService), new Uri(Address)))
            {
                serviceHost.Open();
                Console.WriteLine("Host started");
                Console.WriteLine("Press Enter to terminate the host...");
                Console.ReadLine();
            }
        }
    }

    public class TaskManagerModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository>().To<MemoRepository>();
            Bind<ITaskFactory>().To<TaskFactory>();
            Bind<IToDoList>().To<ToDoList>();
        }
    }

    public class TaskManagerService : ITaskManagerService
    {
        private readonly TaskManagerModule module;
        private readonly IKernel kernel;
        private readonly IToDoList taskList;

        public TaskManagerService()
        {
            module = new TaskManagerModule();
            kernel = new StandardKernel(module);
            taskList = kernel.Get<ToDoList>();

            Console.WriteLine("added new task");
        }

        public ContractTask AddTask(ContractTask task)
        {
            var sTask = taskList.AddTask(task);
<<<<<<< HEAD
            return new ContractTask() { Id = sTask.Id, Name = sTask.Name };
=======
            return new ContractTask() {Id = sTask.Id, Name = sTask.Name};
        }
    }


    public class TaskManagerServiceTests
    {
        private readonly ContractTask incomingTask = new ContractTask();
        private readonly ContractTask outgoingTask = new ContractTask();
        private readonly IToDoList list = Substitute.For<IToDoList>();
        private readonly ITaskManagerService manager;

        public TaskManagerServiceTests()
        {
            manager = new TaskManagerService();         
        }

        [Fact]
        public void should_send_and_return_task()
        {
            //arrange
            list.AddTask(outgoingTask).Returns(incomingTask);

            var task = manager.AddTask(outgoingTask);

            task.Should().Be(incomingTask);
>>>>>>> master
        }
    }
}