using DAL.DTO;
using System.Windows.Forms;

namespace PersonalTracking.ScreenNotifications.DepartmentNotifications
{
    public class DeparmentInfo
    {

        public static void InvalidMinimumAmountDepartmentCharacters() => MessageBox.Show("The department name is incorrect because the minimum amount of characters is 3.",
                                                                                         "Warning!!",
                                                                                         MessageBoxButtons.OK,
                                                                                         MessageBoxIcon.Warning);

        public static void DeparmentFieldIsEmpty() => MessageBox.Show("Please fill the deparment name field",
                                                                      "Warning!!",
                                                                      MessageBoxButtons.OK,
                                                                      MessageBoxIcon.Warning);

        public static void DeparmentSavedWithSuccess(DepartmentDTO department) => MessageBox.Show($"The deparment {department.DepartmentName} was successfully saved",
                                                                                                  "Success!",
                                                                                                  MessageBoxButtons.OK,
                                                                                                  MessageBoxIcon.Information);

        public static void InvalidDepartmentSelected() => MessageBox.Show("Please select a department from the table.",
                                                                          "Warning!!",
                                                                          MessageBoxButtons.OK,
                                                                          MessageBoxIcon.Warning);
    }
}
