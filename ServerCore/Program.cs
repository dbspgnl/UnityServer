using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
	class SpinLock
	{
		volatile int _locked = 0;
		public void Acquire()
		{
			while (true)
			{
				// 1번 방법
				//	int original = Interlocked.Exchange(ref _locked, 1); // lock하고 0을 1로 변경
				//	if (original == 0) break;

				// CAS Compare-And-Swap
				int expected = 0;
				int desired = 1;
				if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
					break; // 0이면 1로 처리 = if (_locked == 0) _locked = 1;

			}
		}
		public void Release()
		{
			_locked = 0;
		}

	}
	class Program
	{
		static int _num = 0;
		static SpinLock _lock = new SpinLock();

		static void Thread_1()
		{
			for (int i = 0; i < 100000; i++)
			{
				_lock.Acquire();
				_num++;
				_lock.Release();
			}
		}
		static void Thread_2()
		{
			for (int i = 0; i < 100000; i++)
			{
				_lock.Acquire();
				_num--;
				_lock.Release();
			}
		}

		static void Main(string[] args)
		{
			Task t1 = new Task(Thread_1);
			Task t2 = new Task(Thread_2);
			t1.Start();
			t2.Start();

			Task.WaitAll(t1, t2);
			Console.WriteLine(_num);
		}
	}
}
