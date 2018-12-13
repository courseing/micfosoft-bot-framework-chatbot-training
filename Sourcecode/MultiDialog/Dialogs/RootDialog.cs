using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MultiDialog.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string CorporateOption = "Corporate";
        private const string OnlineOption = "Online";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            this.ShowOptions(context);
            //// Calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;
            //// Return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            //context.Wait(MessageReceivedAsync);
        }

        private void ShowOptions(IDialogContext context)
        {
            //List<string> lst = new List<string>() { CorporateOption, OnlineOption };
            PromptDialog.Choice(context, this.OptionSelected, new List<string>() { CorporateOption, OnlineOption }, "Are you looking for Corporate or Online training?", "Not a valid options", 3);
        }

        private async Task OptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                switch(optionSelected)
                {
                    case OnlineOption:
                        context.Call(new OnlineDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case CorporateOption:
                        context.Call(new CorporateDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch(Exception e)
            {
                await context.PostAsync("Thanks");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }
    }
}