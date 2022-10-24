using Microsoft.AspNetCore.Mvc.ModelBinding;
using Ratings.Models;
using System.Globalization;

namespace Ratings.ModelBinders
{
    public class CustomRatingModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            int id = int.Parse(bindingContext.ValueProvider.GetValue("Id").First());
            string review = bindingContext.ValueProvider.GetValue("Review").First();
            int workId = int.Parse(bindingContext.ValueProvider.GetValue("WorkId").First());
            string ratingValStr = bindingContext.ValueProvider.GetValue("RatingValue").FirstOrDefault();
            decimal ratingValue = 0;
            if (!string.IsNullOrEmpty(ratingValStr))
            {
                ratingValue = decimal.Parse(ratingValStr, CultureInfo.CurrentCulture);
            }

            Rating rating = new Rating
            {
                Id = id,
                Review = review,
                WorkId = workId,
                RatingValue = ratingValue
            };

            bindingContext.Result = ModelBindingResult.Success(rating);
            return Task.CompletedTask;
        }
    }
}
