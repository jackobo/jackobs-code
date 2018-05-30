using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IComponentPublisher
    {
        Optional<VersionNumber> GetCurrentVersion();
        INextVersionProvider GetNextVersionProvider();
        void AcceptCommandVisitor(Func<IComponentPublisherVisitor> visitorFactory);
        IEnumerable<StringKeyValue> GetMetadata();
    }

    public interface INextVersionProvider
    {
        
        Optional<VersionNumber> NextRegularVersion { get; }
        Optional<VersionNumber> NextMinorVersion { get; }
    }

    public interface IComponentPublisherVisitor
    {
        void Visit(ICoreComponentPublisher coreComponentPublisher);
        void Visit(IGameEnginePublisher gameEnginePublisher);
        void Visit(IGameMathPublisher gameMathPublisher);
        void Visit(IGameLimitsPublisher gameLimistPublisher);
    }


    public interface IComponentPublisherVisitor<T> : IComponentPublisherVisitor
    {
        T ProduceResult();
    }

    public interface ICoreComponentPublisher : IComponentPublisher
    {
        string Name { get; }
    }

    public interface IGameEnginePublisher : IComponentPublisher
    {
        GameEngineName EngineName { get; }
    }
  
    public interface IGameMathPublisher : IComponentPublisher
    {
        GameEngineName EngineName { get; }
        string GameName { get; }
    }

    public interface IGameLimitsPublisher : IComponentPublisher
    {
        GameEngineName EngineName { get; }
        string GameName { get; }
    }

}
