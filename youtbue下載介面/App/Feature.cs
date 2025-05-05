using youtbue下載介面.Models;
namespace youtbue下載介面.App
{
    internal class Feature
    {
        public Action action{get;set;}
        public Action error_action {get;set;}
        Queue<string> _alert_msg;

        public string successMessage {get;set;}
        public string errorMessage {get;set;}
        public Feature nextFeature{ get;set;}
        
        public void Start()
        {
            while (true)
            {
                try
                {
                    action?.Invoke();
                    if ( nextFeature != null) 
                        nextFeature.Start();
                    else
                        if(successMessage != null)
                            Console.WriteLine(successMessage);
                        return ;
                }
                catch (Exception e)
                {
                    Console.WriteLine("錯誤 :" + e.Message);
                    if(errorMessage != null)
                        Console.WriteLine(errorMessage);
                    error_action?.Invoke();
                }



                Console.WriteLine("重新開始動作？(y/n)");
                if( Console.ReadLine() != "y")
                {
                    break;
                }

            }
        }

    }
}