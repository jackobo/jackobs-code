using System;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Synchronizers
{
    public interface IPendingGame
    {
        void SynchronizeGame(IComponentSynchronizer gameSynchronizer);
        string GetDescription();
    }

    public class PendingGame : IPendingGame
    {
        public PendingGame(int gameType, IArtifactoryRepositoryDescriptor repositoryDescriptor)
        {
            GameType = gameType;
            RepositoryDescriptor = repositoryDescriptor;
        }

        public int GameType { get; private set; }
        public IArtifactoryRepositoryDescriptor RepositoryDescriptor { get; private set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as PendingGame;

            if (theOther == null)
                return false;

            return this.GameType == theOther.GameType
                    && this.RepositoryDescriptor.Equals(theOther.RepositoryDescriptor);
        }

        public override int GetHashCode()
        {
            return this.GameType.GetHashCode()
                    ^ this.RepositoryDescriptor.GetHashCode();
        }

        public void SynchronizeGame(IComponentSynchronizer gameSynchronizer)
        {
            gameSynchronizer.SynchronizeComponent(RepositoryDescriptor, GameType);
        }


        public string GetDescription()
        {
            return $"Game Type = {GameType}; Repository = {RepositoryDescriptor.Repository.GetRootFolderRelativeUrl()}";
        }


        public override string ToString()
        {
            return $"GameType = {this.GameType}; {this.RepositoryDescriptor.ToString()}";
        }
    }
}