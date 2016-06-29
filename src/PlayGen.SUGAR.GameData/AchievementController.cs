﻿using System.Linq;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.EntityFramework.Interfaces;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.GameData
{
	public class AchievementController : DataEvaluationController
	{
		protected readonly RewardController RewardController;

		public AchievementController(IGameDataController gameDataController)
			: base(gameDataController)
		{
			RewardController = new RewardController(gameDataController);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="achievement"></param>
		/// <param name="actorId"></param>
		/// <returns></returns>
		public bool IsAchievementCompleted(Achievement achievement, int? actorId)
		{
			if (achievement == null)
			{
				throw new MissingRecordException("The provided achievement does not exist.");
			}
			var key = string.Format(KeyConstants.AchievementCompleteFormat, achievement.Token);
			var completed =  GameDataController.KeyExists(achievement.GameId, actorId, key);

			if (!completed)
			{
				completed = IsCriteriaSatisified(achievement.GameId, actorId, achievement.CompletionCriteriaCollection);
				if (completed)
				{
					ProcessAchievementRewards(achievement, actorId);
				}
			}

			return completed;
		}

		private void ProcessAchievementRewards(Achievement achievement, int? actorId)
		{
			var gameData = new Data.Model.GameData()
			{
				Key = string.Format(KeyConstants.AchievementCompleteFormat, achievement.Token),
				GameId = achievement.GameId,    //TODO: handle the case where a global achievement has been completed for a specific game
				ActorId = actorId,
				DataType = GameDataType.String,
				Value = null
			};
			GameDataController.Create(gameData);
			achievement.RewardCollection.All(reward => RewardController.AddReward(actorId, achievement.GameId, reward));
		}

		public void EvaluateAchievement(Achievement achievement, int? actorId)
		{
		}
	}
}