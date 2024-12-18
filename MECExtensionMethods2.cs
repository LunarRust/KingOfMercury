using System.Collections.Generic;
using System.Threading;
using Godot;
using MEC;

public static class MECExtensionMethods2
{
	public static IEnumerator<double> CancelWith(this IEnumerator<double> coroutine, Node node)
	{
		while (Timing.MainThread != Thread.CurrentThread || (IsNodeAlive(node) && coroutine.MoveNext()))
		{
			yield return coroutine.Current;
		}
	}

	public static IEnumerator<double> CancelWith(this IEnumerator<double> coroutine, Node node1, Node node2)
	{
		while (Timing.MainThread != Thread.CurrentThread || (IsNodeAlive(node1) && IsNodeAlive(node2) && coroutine.MoveNext()))
		{
			yield return coroutine.Current;
		}
	}

	public static IEnumerator<double> CancelWith(this IEnumerator<double> coroutine, Node node1, Node node2, Node node3)
	{
		while (Timing.MainThread != Thread.CurrentThread || (IsNodeAlive(node1) && IsNodeAlive(node2) && IsNodeAlive(node3) && coroutine.MoveNext()))
		{
			yield return coroutine.Current;
		}
	}

	private static bool IsNodeAlive(Node node)
	{
		return node != null && !node.IsQueuedForDeletion() && node.IsInsideTree();
	}
}
