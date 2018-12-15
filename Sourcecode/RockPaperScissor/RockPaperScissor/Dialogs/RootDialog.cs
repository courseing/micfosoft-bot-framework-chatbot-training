using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RockPaperScissor.Common;
using RockPaperScissor.Model;
using RockPaperScissor.ViewModel;

namespace RockPaperScissor.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        bool userWelcomed = false;
        Game game;

        public RootDialog()
        {
            game = new Game();
        }

        public async Task StartAsync(IDialogContext context)
        {

            string name, message;
            if (context.UserData.TryGetValue(StateKeys.UserName, out name))
            {
                message = $"Hello {name}, Welcome Back to the Game!";
                await context.PostAsync(message);
            }


            context.Wait(MessageReceivedAsync);

            //return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            

            string name;
            string message="";
            if (!context.UserData.TryGetValue(StateKeys.UserName, out name))
            {
                //playScore.UserScore = 0;
                //playScore.BotScore = 0;
                message = "Hello There, Welcome to the Game! \n What's you Name?";
                PromptDialog.Text(context, this.ResumeAfterPrompt, message);
                return;
            }
            string gameResult = game.Play(activity.Text);
            game.score.BotScore++;
            context.ConversationData.SetValue<PlayScore>(StateKeys.GameScore, game.score);

            if (context.ConversationData.TryGetValue<PlayScore>(StateKeys.GameScore, out game.score))
            {
                message = $"Your score : {game.score.UserScore}, Bot Score : {game.score.BotScore}";
            }

            // Return our reply to the user
            await context.PostAsync(gameResult + "\n" + message);

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var userName = await result;
                this.userWelcomed = true;

                await context.PostAsync($"Welcome {userName} !");

                context.UserData.SetValue(StateKeys.UserName, userName);
            }
            catch (TooManyAttemptsException)
            {
            }
            context.Wait(this.MessageReceivedAsync);
        }

    }
}