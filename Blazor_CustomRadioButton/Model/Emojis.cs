using System.ComponentModel.DataAnnotations;

namespace Blazor_CustomRadioButton.Model
{
    public class Emoji
    {
        public EmojiEnum EmojiOptions = EmojiEnum.DB01;
        
        public enum EmojiEnum
        {
            [Display(Name="Happy")]
            MM01,
            [Display(Name="Angry")]
            DB01
        }
    }
}