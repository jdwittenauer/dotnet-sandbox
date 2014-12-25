using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sandbox.Reactive.DictionaryService;

namespace Sandbox.Reactive
{
    /// <summary>
    /// Learning exercises for the reactive extensions library.
    /// </summary>
    public static class RxExamples
    {
        public static void Run()
        {
            Console.WriteLine("Reactive Extensions examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            Exercise1();
            Exercise2();
            Exercise3();
            Exercise4();
            Exercise5();

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static void Exercise1()
        {
            // Generator
            IObservable<int> source = Observable.Generate(
                0,
                i => i < 5,
                i => i + 1,
                i => i * i);
            IDisposable subscription = source.Subscribe(
                x => Console.WriteLine("OnNext:  {0}", x),
                ex => Console.WriteLine("OnError: {0}", ex),
                () => Console.WriteLine("OnCompleted"));

            Console.WriteLine("Press ENTER to unsubscribe...");
            Console.ReadLine();
            subscription.Dispose();

            // Time-based generator
            IObservable<int> source2 = Observable.Generate(
                0,
                i => i < 5,
                i => i + 1,
                i => i * i,
                i => TimeSpan.FromSeconds(i));
            IDisposable subscription2 = source.Subscribe(
                x => Console.WriteLine("OnNext:  {0}", x),
                ex => Console.WriteLine("OnError: {0}", ex),
                () => Console.WriteLine("OnCompleted"));

            Console.WriteLine("Press ENTER to unsubscribe...");
            Console.ReadLine();
            subscription2.Dispose();

            Console.WriteLine("Exercise 1 complete.");
            Console.ReadLine();
        }

        private static void Exercise2()
        {
            // Capture mouse events as an observable
            var label = new Label();
            var form = new Form
            {
                Controls = { label }
            };

            var moves = Observable.FromEventPattern<MouseEventArgs>(form, "MouseMove");
            using (moves.Subscribe(e => { label.Text = e.EventArgs.Location.ToString(); }))
            {
                Application.Run(form);
            }

            Console.WriteLine("Exercise 2 complete.");
            Console.ReadLine();
        }

        private static void Exercise3()
        {
            // Same as exercise 2 but using LINQ on the event stream
            var label = new Label();
            var form = new Form
            {
                Controls = { label }
            };

            var moves = Observable.FromEventPattern<MouseEventArgs>(form, "MouseMove")
                .Select(e => e.EventArgs.Location).Where(e => e.X == e.Y);
            var movesSubscription = moves.Subscribe(pos => Console.WriteLine("Mouse at: " + pos));

            Application.Run(form);

            Console.WriteLine("Exercise 3 complete.");
            Console.ReadLine();
        }

        private static void Exercise4()
        {
            // Text box example demonstrating several new operators
            var text = new TextBox();
            var form = new Form
            {
                Controls = { text }
            };

            var input = Observable.FromEventPattern<EventArgs>(text, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Throttle(TimeSpan.FromSeconds(1))
                .Do(i => Console.WriteLine("Before DistinctUntilChanged: " + i))
                .DistinctUntilChanged();

            using (input.Subscribe(i => Console.WriteLine("User wrote: " + i)))
            {
                Application.Run(form);
            }

            Console.WriteLine("Exercise 4 complete.");
            Console.ReadLine();
        }

        private static void Exercise5()
        {
            // Complete example using an external web service as an observable collection
            var text = new TextBox();
            var listBox = new ListBox { Top = text.Height + 10 };
            var form = new Form { Controls = { text, listBox } };
            
            // Turn the user input into a tamed sequence of strings
            var textChanged = from evt in Observable.FromEventPattern<EventArgs>(text, "TextChanged") 
                              select ((TextBox)evt.Sender).Text;

            var input = textChanged
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged();
            
            // Bridge with the web service's MatchInDict method
            var service = new DictServiceSoapClient("DictServiceSoap");
            var matchInDict = Observable.FromAsyncPattern<string, string, string, DictionaryWord[]>
                (service.BeginMatchInDict, service.EndMatchInDict);

            Func<string, IObservable<DictionaryWord[]>> matchInWordNetByPrefix = 
                term => matchInDict("wn", term, "prefix");
            
            // The grand composition connecting the user input with the web service
            var result = (from term in input
                          select matchInWordNetByPrefix(term))
                      .Switch();
            
            // Synchronize with the UI thread and populate the ListBox or signal an error
            
            using (result.ObserveOn(listBox).Subscribe(
                words => { 
                    listBox.Items.Clear();
                    listBox.Items.AddRange((from word in words select word.Word).ToArray());
                }, 
                ex => { 
                    MessageBox.Show("An error occurred: " + ex.Message, form.Text, 
                        MessageBoxButtons.OK, MessageBoxIcon.Error); 
                })) 
            { 
                Application.Run(form);
            }
        }
    }
}
