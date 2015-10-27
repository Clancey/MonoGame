using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace Microsoft.Xna.Framework.Input
{
    public static partial class MessageBox
    {
        private static TaskCompletionSource<int?> tcs;
		#if !__TVOS__
        private static UIAlertView alert;
		#endif

        private static Task<int?> PlatformShow(string title, string description, List<string> buttons)
        {
			#if __TVOS__
			throw new NotImplementedException();
			#else
            tcs = new TaskCompletionSource<int?>();
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                alert = new UIAlertView();
                alert.Title = title;
                alert.Message = description;
                foreach (string button in buttons)
                    alert.AddButton(button);
                alert.Dismissed += (sender, e) =>
                {
                    if (!tcs.Task.IsCompleted)
					    tcs.SetResult((int)e.ButtonIndex);
                };
                alert.Show();
            });
			#endif

            return tcs.Task;
        }

        private static void PlatformCancel(int? result)
        {
            if (!tcs.Task.IsCompleted)
                tcs.SetResult(result);
			#if !__TVOS__
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                alert.DismissWithClickedButtonIndex(0, true);
            });
			#endif
        }
    }
}
