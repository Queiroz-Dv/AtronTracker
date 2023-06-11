using System.Windows.Forms;
using static PersonalTracking.Helpers.MessageHelper;

namespace PersonalTracking.ScreenNotifications
{
    public class InfoMessages
    {

        public static void InvalidMinimumAmountCharacters(bool condition, string fieldName) => ShowMessage(condition,
                                                                                                           $"The name of {fieldName} is incorrect because the minimum amount of characters is 3.",
                                                                                                           MessageBoxButtons.OK,
                                                                                                           EnumLevelMessage.Warning);

        public static void FieldIsEmpty(bool condition, string fieldName) => ShowMessage(condition,
                                                                       $"Please fill the {fieldName}",
                                                                       MessageBoxButtons.OK,
                                                                       EnumLevelMessage.Warning);

        public static void EntitySavedWithSuccess(object entity) => MessageBox.Show($"The {entity} was successfully saved",
                                                                                                  "Success!",
                                                                                                  MessageBoxButtons.OK,
                                                                                                  MessageBoxIcon.Information);

        public static void EntityUpdated(object entity) => MessageBox.Show($"{entity} was updated successfully!",
                                                                           "Updated!",
                                                                           MessageBoxButtons.OK,
                                                                           MessageBoxIcon.Information);

        public static void InvalidItemSelected() => MessageBox.Show("Please select an item from the table.",
                                                                          "Warning!!",
                                                                          MessageBoxButtons.OK,
                                                                          MessageBoxIcon.Warning);

        public static DialogResult UpdatedEntityQuestion(bool condition, string fieldName) => ShowMessage(condition,
                                                                                                          $"Are you sure to change the {fieldName}?",
                                                                                                          MessageBoxButtons.YesNo,
                                                                                                          EnumLevelMessage.Question);

        public static DialogResult DeleteEntityQuestion(bool condition, string fieldName) => ShowMessage(condition, $"Are you sure to delete this {fieldName}?",
                                                                                          MessageBoxButtons.YesNo,
                                                                                          EnumLevelMessage.Question);

        public static void EntityDeletedWithSuccess(bool condition, string fieldName) => ShowMessage(condition,
                                                                                                     $"{fieldName} was deleted successfully",
                                                                                                     MessageBoxButtons.OK,
                                                                                                     EnumLevelMessage.Information);
    }
}