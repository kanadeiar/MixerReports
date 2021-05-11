using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MixerReports.lib.Models
{
    public partial class Mix : IDataErrorInfo
    {
        [NotMapped] public string NormalStr => (Normal) ? "Норма" : "Ошибка";

        [NotMapped] public string UndersizedStr => (Undersized) ? "Недоросток" : String.Empty;

        [NotMapped] public string OvergroundStr => (Overground) ? "Переросток" : string.Empty;

        [NotMapped] public string BoiledStr => (Boiled) ? "Закипел" : string.Empty;

        [NotMapped] public string OtherStr => (Other) ? "Другое" : string.Empty;

        [NotMapped] public string IsMudStr => (IsMud) ? "Сброс в шлам" : string.Empty;

        [NotMapped] public string IsExperimentStr => (IsExperiment) ? "Эксперимент" : string.Empty;

        /// <summary> Характеристики в виде строки </summary>
        [NotMapped]
        public string CharsStr
        {
            get
            {
                List<string> strs = new List<string>();
                if (Undersized) strs.Add(UndersizedStr);
                if (Overground) strs.Add(OvergroundStr);
                if (Boiled) strs.Add(BoiledStr);
                if (Other) strs.Add(OtherStr);
                if (IsMud) strs.Add(IsMudStr);
                if (IsExperiment) strs.Add(IsExperimentStr);
                if (strs.Count > 1)
                    return strs.Aggregate((m, n) => m + ',' + n);
                else if (strs.Count > 0)
                    return strs.First();
                else
                    return string.Empty;
            }
        }

        #region Валидация

        [NotMapped]
        string IDataErrorInfo.Error => null;

        [NotMapped]
        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case nameof(FormNumber):
                        var formNumber = FormNumber;
                        if (formNumber < 0) return "Номер формы не может быть отрицательным числом";
                        if (formNumber > 100) return "Номер формы не может быть больше 100";
                        return null;
                    case nameof(RecipeNumber):
                        var recipeNumber = RecipeNumber;
                        if (recipeNumber < 0) return "Номер рецепта не может быть отрицательным числом";
                        if (recipeNumber > 100_000) return "Номер рецепта не может быть больше 100 тысяч";
                        return null;
                    case nameof(Comment):
                        var comment = Comment;
                        if (comment?.Length >= 250) return "Комментарий к заливке не может быть длиннее 250 символов";
                        return null;
                    default:
                        return null;
                }
            }
        }

        #endregion
    }
}
