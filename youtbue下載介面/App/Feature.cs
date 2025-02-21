using youtbue下載介面.Models;
namespace youtbue下載介面.App
{
    internal class Feature
    {
        public Action action{get;set;}
        public Action error_action {get;set;}
        Queue<string> _alert_msg;
        public Feature nextFeature{ get;set;}
        
        public void Start()
        {
            while (true)
            {
                try
                {
                    action?.Invoke();
                    if ( nextFeature != null) nextFeature.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("錯誤 :" + e.Message);
                    error_action?.Invoke();
                }

                Console.WriteLine("請問是否重新開始動作？(y/n)");
                if( Console.ReadLine() != "y")
                {
                    break;
                }

            }
        }

    }
}