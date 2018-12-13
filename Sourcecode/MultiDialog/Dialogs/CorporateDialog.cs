using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MultiDialog.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiDialog.Dialogs
{

    [Serializable]
    internal class CorporateDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("Enter Course Name");
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        public static async Task<string> IdentifyCourseUsingLuis(string message)
        {
            string course = "";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/59583dde-cc2f-4554-b004-db3bae7e9b8a?subscription-key=4939782dc2df43fbb61a67154a79a30a&staging=true&verbose=true&timezoneOffset=-360&q="
                    + System.Uri.EscapeDataString(message));
                    dynamic response = JObject.Parse(responseInString);

                    var intent = response.intents?.First?.intent;

                    if (intent == "getcoursedetails")
                    {
                        foreach (var entity in response.entities)
                        {
                            course = entity.entity;
                            break;
                        }
                    }
                }
                catch(Exception e)
                {
                    return "Failed to process";
                }
            }
            return course;
        }

        private Task RequestForInput(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Text(context, this.MessageReceivedAsync, "Enter Hotel Name");
            return null;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            
            var searchQuery = await result as Activity;
            string query = searchQuery.Text;

            //var coursesEnq = await GetEntityFromConginitiveService(query);


            await context.PostAsync($"You enquired for {searchQuery}");

            var courses = await this.GetCoursesAsync(query);

            await context.PostAsync($"No of Courses found : {courses.Count()}");

            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            foreach(var course in courses)
            {
                HeroCard heroCard = new HeroCard()
                {
                    Title = course.Name,
                    Subtitle = $"{course.Rating} starts. {course.NumberOfReviews} reviews. From ${course.PriceStarting} per night.",
                    Images = new List<CardImage>()
                    {
                        new CardImage(){Url = course.Image}
                    },
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = "More Details",
                            Type = ActionTypes.OpenUrl,
                            Value = $"https://www.bing.com/search?q=courses+in+" + HttpUtility.UrlEncode(course.Name)
                        }
                    }
                };
                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            await context.PostAsync(resultMessage);
        }

        private static async Task<LuisResponse> GetEntityFromConginitiveService(string text)
        {
            LuisResponse Data = new LuisResponse();
            try
            {
                text = Uri.EscapeDataString(text);

                using (HttpClient client = new HttpClient())
                {
                    var responseInString = await client.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/59583dde-cc2f-4554-b004-db3bae7e9b8a?subscription-key=4939782dc2df43fbb61a67154a79a30a&staging=true&verbose=true&timezoneOffset=-360&q="
                   + System.Uri.EscapeDataString(text));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);

                    //if (msg.IsSuccessStatusCode)
                    //{
                    //    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                        
                    //}
                }
            }
            catch (Exception ex)
            {
                //ErrorLog.ErrorLogging("", ex.Message, "", DateTime.Now);
                throw ex;
            }
            return Data;
        }

        //private static async Task<Rootobject> GetEntityFromConginitiveService_Old(string text)
        //{
        //    Rootobject Data = new Rootobject();
        //    try
        //    {
        //        text = Uri.EscapeDataString(text);

        //        using (HttpClient client = new HttpClient())
        //        {
        //            string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/be4de1f5-e380-4995-b746-769f3f944755?subscription-key=9de04f51e2d64fe8bd7b26c720baa9d0&timezoneOffset=-360&q=" + text;
        //            HttpResponseMessage msg = await client.GetAsync(RequestURI);

        //            if (msg.IsSuccessStatusCode)
        //            {
        //                var JsonDataResponse = await msg.Content.ReadAsStringAsync();
        //                Data = JsonConvert.DeserializeObject<Rootobject>(JsonDataResponse);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.ErrorLogging("", ex.Message, "", DateTime.Now);
        //        throw ex;
        //    }
        //    return Data;
        //}

        private async Task<IEnumerable<Course>> GetCoursesAsync(string searchQuery)
        {
            var courses = new List<Course>();

            // Filling the courses results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Course course = new Course()
                {
                    Name = $"{searchQuery} + Course {i}",
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt={searchQuery}course+{i}&w=500&h=260"
                };
                courses.Add(course);
            }

            courses.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));
            return courses;
        }
    }
}