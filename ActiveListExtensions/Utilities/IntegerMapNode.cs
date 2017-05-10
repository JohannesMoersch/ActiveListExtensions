using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	public class IntegerMapNode
	{
		public enum IntegerMapOffsetType
		{
			LessThanDivider,
			GreaterThanOrEqualToDivider
		}

		public static IntegerMapNode CreateRoot() => new IntegerMapNode();

		public int? Divider { get; private set; }

		public int Offset { get; private set; }

		public IntegerMapNode LowerNode { get; private set; }

		public IntegerMapNode UpperNode { get; private set; }

		private bool CanReduce => Offset == 0 && (LowerNode?.Offset ?? 0) == (UpperNode?.Offset ?? 0) && LowerNode?.UpperNode == null && UpperNode?.LowerNode == null;

		private IntegerMapNode()
		{
		}

		public int GetOffset(int target)
		{
			if (!Divider.HasValue)
				return Offset;
			if (target < Divider.Value)
				return Offset + (LowerNode?.GetOffset(target) ?? 0);
			return Offset + (UpperNode?.GetOffset(target) ?? 0);
		}

		public void Update(int divider, int offset, IntegerMapOffsetType type)
		{
			if (offset == 0)
				return;

			if (!Divider.HasValue)
				Divider = divider;

			if (divider < Divider.Value)
				UpdateLower(divider, offset, type);
			else if (divider > Divider.Value)
				UpdateUpper(divider, offset, type);
			else if (type == IntegerMapOffsetType.LessThanDivider)
				AddToLower(offset);
			else
				AddToUpper(offset);
		}

		private void AddToLower(int offset)
		{
			if (LowerNode == null)
				LowerNode = new IntegerMapNode() { Offset = offset };
			else
			{
				LowerNode.Offset += offset;
				LowerNode = LowerNode.Reduce();
			}
		}

		private void AddToUpper(int offset)
		{
			if (UpperNode == null)
				UpperNode = new IntegerMapNode() { Offset = offset };
			else
			{
				UpperNode.Offset += offset;
				UpperNode = UpperNode.Reduce();
			}
		}

		private void UpdateLower(int divider, int offset, IntegerMapOffsetType type)
		{
			if (LowerNode == null)
				LowerNode = new IntegerMapNode();

			LowerNode.Update(divider, offset, type);

			if (type == IntegerMapOffsetType.GreaterThanOrEqualToDivider)
				AddToUpper(offset);
		}

		private void UpdateUpper(int divider, int offset, IntegerMapOffsetType type)
		{
			if (UpperNode == null)
				UpperNode = new IntegerMapNode();

			UpperNode.Update(divider, offset, type);

			if (type == IntegerMapOffsetType.LessThanDivider)
				AddToLower(offset);
		}

		private IntegerMapNode Reduce()
		{
			if (!CanReduce)
				return this;

			if (LowerNode == null)
				return UpperNode;

			if (UpperNode == null)
				return LowerNode;

			LowerNode.UpperNode = UpperNode;

			return LowerNode;
		}
	}
}
