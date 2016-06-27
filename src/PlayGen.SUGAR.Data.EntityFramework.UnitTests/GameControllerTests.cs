﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using Xunit;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
	public class GameControllerTests : TestController
	{
		#region Configuration
		private readonly GameController _gameDbController;

		public GameControllerTests()
		{
			_gameDbController = new GameController(NameOrConnectionString);
		}
		#endregion


		#region Tests
		[Fact]
		public void CreateAndGetGame()
		{
			string gameName = "CreateGame";

			CreateGame(gameName);
			
			var games = _gameDbController.Search(gameName);

			int matches = games.Count(g => g.Name == gameName);

			Assert.Equal(1, matches);
		}

		[Fact]
		public void CreateDuplicateGame()
		{
			string gameName = "CreateDuplicateGame";

			CreateGame(gameName);
			
			bool hadDuplicateException = false;

			try
			{
				CreateGame(gameName);
			}
			catch (DuplicateRecordException)
			{
				hadDuplicateException = true;
			}

			Assert.True(hadDuplicateException);
		}
		
		[Fact]
		public void GetMultipleGames()
		{
			string[] gameNames = new[]
			{
				"GetMultipleGames1",
				"GetMultipleGames2",
				"GetMultipleGames3",
				"GetMultipleGames4",
			};

			foreach (var gameName in gameNames)
			{
				CreateGame(gameName);
			}

			CreateGame("GetMultiple_Games_DontGetThis");

			var games = _gameDbController.Search("GetMultipleGames");

			var matchingGames = games.Select(g => gameNames.Contains(g.Name));
			
			Assert.Equal(gameNames.Length, matchingGames.Count());
		}

		[Fact]
		public void GetNonExistingGame()
		{
			var games = _gameDbController.Search("GetNonExsitingGame");

			Assert.Empty(games);
		}

		[Fact]
		public void DeleteExistingGame()
		{
			string gameName = "DeleteExistingGame";

			var game = CreateGame(gameName);

			var games = _gameDbController.Search(gameName);
			Assert.Equal(games.Count(), 1);
			Assert.Equal(games.ElementAt(0).Name, gameName);

			_gameDbController.Delete(new []{game.Id});
			games = _gameDbController.Search(gameName);

			Assert.Empty(games);
		}

		[Fact]
		public void DeleteNonExistingGame()
		{
			bool hadException = false;

			try
			{
				_gameDbController.Delete(new int[] {-1});
			}
			catch (Exception)
			{
				hadException = true;
			}

			Assert.False(hadException);
		}
		#endregion

		#region Helpers
		private Game CreateGame(string name)
		{
			var game = new Game
			{
				Name = name,
			};

			_gameDbController.Create(game);

			return game;
		}
		#endregion
	}
}
