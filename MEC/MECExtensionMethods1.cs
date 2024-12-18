using System.Collections.Generic;

namespace MEC;

public static class MECExtensionMethods1
{
	public static CoroutineHandle RunCoroutine(this IEnumerator<double> coroutine)
	{
		return Timing.RunCoroutine(coroutine);
	}

	public static CoroutineHandle RunCoroutine(this IEnumerator<double> coroutine, string tag)
	{
		return Timing.RunCoroutine(coroutine, tag);
	}

	public static CoroutineHandle RunCoroutine(this IEnumerator<double> coroutine, Segment segment)
	{
		return Timing.RunCoroutine(coroutine, segment);
	}

	public static CoroutineHandle RunCoroutine(this IEnumerator<double> coroutine, Segment segment, string tag)
	{
		return Timing.RunCoroutine(coroutine, segment, tag);
	}
}
