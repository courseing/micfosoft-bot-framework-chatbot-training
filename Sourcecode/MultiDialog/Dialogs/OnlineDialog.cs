using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MultiDialog.Dialogs
{
    internal class OnlineDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}