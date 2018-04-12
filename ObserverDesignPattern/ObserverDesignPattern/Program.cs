using System;
using System.Collections.Generic;

namespace ObserverDesignPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            BaggageHandler provider = new BaggageHandler();
            ArrivalsMontior observer1 = new ArrivalsMontior("BaggageClaimMonitor1");
            ArrivalsMontior observer2 = new ArrivalsMontior("SecurityExit");

            provider.BaggageStatus(712, "Detroit", 3);
            observer1.Subscribe(provider);
            observer2.Subscribe(provider);
            provider.BaggageStatus(400, "New York-Kennedy", 1);
            observer2.Unsubscribe();
            provider.LastBaggageClaimed();
        }
    }

    public class BaggageInfo
    {
        private int flightNo;
        private string origin;
        private int location;

        public BaggageInfo(int flight, string from, int carousel)
        {
            this.flightNo = flight;
            this.origin = from;
            this.location = carousel;
        }

        public int FlightNumber
        {
            get { return this.flightNo; }
        }

        public string From
        {
            get { return this.origin; }
        }

        public int Carousel
        {
            get { return this.location; }
        }
    }

    public class BaggageHandler : IObservable<BaggageInfo>
    {
        //Observalbe维护者一个Observer的集合，对，一点没错
        private List<IObserver<BaggageInfo>> observers;
        private List<BaggageInfo> flights;

        public BaggageHandler()
        {
            observers = new List<IObserver<BaggageInfo>>();
            flights = new List<BaggageInfo>();
        }

        /// <summary>
        /// 这里的订阅方法是供Observer调用的
        /// 订阅的本质就是把观察者加入当前的集合中，并告知观察者的当前信息
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<BaggageInfo> observer)
        {
            if(!observers.Contains(observer))
            {
                observers.Add(observer);

                foreach(var item in flights)
                {
                    observer.OnNext(item);
                }
            }

            //也就是在订阅的时候就定义了disposable的方式，本质是把当前观察者将来能从所有观察者集合中移除
            return new Unsubscriber<BaggageInfo>(observers, observer);
        }

        /// <summary>
        /// 告诉外界这个航班号取行李结束，通知所有的观察者
        /// </summary>
        /// <param name="flightNo"></param>
        public void BaggageStatus(int flightNo)
        {
            BaggageStatus(flightNo, string.Empty, 0);
        }

        /// <summary>
        /// 航班动态是要告诉所有观察者的
        /// 所以这里的本质是：当有一个新的航班信息加入这里的集合，就告诉所有观察者
        /// 而这里还有一个功能：当要把某个航班信息从集合中移除的是偶，也要告诉所有观察者
        /// </summary>
        /// <param name="flightNo"></param>
        /// <param name="from"></param>
        /// <param name="carousel"></param>
        public void BaggageStatus(int flightNo, string from, int carousel)
        {
            var info = new BaggageInfo(flightNo, from, carousel);

            //如果航班信息这里还没有，那就把航班信息加入到这里的集合中
            if(carousel>0&&!flights.Contains(info))
            {
                flights.Add(info);

                //需要遍历每个观察者，让他们接受到新的信息，注意，这里是定义
                foreach(var observer in observers)
                {
                    observer.OnNext(info);
                }
            }else if (carousel == 0) //carousel=0实际就是一个指示，就是这个航班的取行李已经结束了，不需要再告诉外界了
            {
                var flightsToRemove = new List<BaggageInfo>();

                //遍历现有的航班
                foreach(var flight in flights)
                {
                    //如果当前的航班号和现有的航班号一致
                    if (info.FlightNumber == flight.FlightNumber)
                    {
                        flightsToRemove.Add(flight);
                        //告知所有的观察者当前航班信息
                        foreach(var observer in observers)
                        {
                            observer.OnNext(info);
                        }
                    }
                }
                foreach(var flightToRemove in flightsToRemove)
                {
                    flights.Remove(flightToRemove);
                }

                flightsToRemove.Clear();
            }
        }


        /// <summary>
        /// 通知所有的观察者，消息推送结束
        /// </summary>
        public void LastBaggageClaimed()
        {
            foreach (var observer in observers)
                observer.OnCompleted();

            observers.Clear();
        }
    }

    public class Unsubscriber<BaggageInfo> : IDisposable
    {
        private List<IObserver<BaggageInfo>> _observers;
        private IObserver<BaggageInfo> _observer;

        public Unsubscriber(List<IObserver<BaggageInfo>> observers, IObserver<BaggageInfo> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }

    public class ArrivalsMontior : IObserver<BaggageInfo>
    {
        private string name;
        private List<string> flightInfos = new List<string>();
        private IDisposable cancellation;
        private string fmt = "{0,-20} {1,5} {2,3}";

        public ArrivalsMontior(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("the observer must be assigned a name");
            this.name = name;
        }

        public virtual void Subscribe(BaggageHandler provider)
        {
            cancellation = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            cancellation.Dispose();
            flightInfos.Clear();
        }

        public void OnCompleted()
        {
            flightInfos.Clear();
        }

        public void OnError(Exception error)
        {
            //do nothing
        }

        public void OnNext(BaggageInfo info)
        {
            bool updated = false; //根据这个字段来决定是否显示信息
            if (info.Carousel == 0)
            {
                var flightsToRemove = new List<string>();
                string flightNo = string.Format("{0,5}",info.FlightNumber);

                foreach(var flightInfo in flightInfos)
                {
                    if (flightInfo.Substring(21, 5).Equals(flightNo))
                    {
                        flightsToRemove.Add(flightInfo);
                        updated = true;
                    }
                }
                foreach(var flightToRemove in flightsToRemove)
                {
                    flightInfos.Remove(flightToRemove);
                }

                flightsToRemove.Clear();
            }
            else
            {
                string flightInfo = String.Format(fmt, info.From, info.FlightNumber, info.Carousel);
                if(!flightInfos.Contains(flightInfo))
                {
                    flightInfos.Add(flightInfo);
                    updated = true;
                }
            }
            if(updated)
            {
                flightInfos.Sort();
                Console.WriteLine("Arrivals information from {0}", this.name);
                foreach(var flightInfo in flightInfos)
                {
                    Console.WriteLine(flightInfo);
                }
                Console.WriteLine();
            }
        }
    }
}
