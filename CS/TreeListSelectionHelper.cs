using System;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes.Operations;

namespace SelectAllAndCopy {
	public class TreeListSelectionHelper {
		public static string GetNodeText(TreeListNode node, TreeListColumn[] columns) {
			const string Delimiter = "\t";
			string result = string.Empty;
			if(node == null) return result;
			foreach(TreeListColumn column in columns) {
				result += node.GetDisplayText(column) + Delimiter;
			}
			if(result.EndsWith(Delimiter))
				result = result.Remove(result.Length - Delimiter.Length, Delimiter.Length);
			return result;
		} 
		public static string GetNodeText(TreeListNode node) {
			if(node == null) return string.Empty;
			TreeList tree = node.TreeList;
			TreeListColumn[] columns = new TreeListColumn[tree.VisibleColumnCount];
			for(int i = 0; i < tree.VisibleColumnCount; i++) {
				columns[i] = tree.GetColumnByVisibleIndex(i);
			}
			return GetNodeText(node, columns);
		}
		public static void CopySelectedToClipboard(TreeList tree) {
			if(tree == null || tree.Selection.Count == 0) return;
			const string LineDelimiter = "\n";
			string nodesText = string.Empty;
			TreeListColumn[] columns = new TreeListColumn[tree.VisibleColumnCount];
			for(int i = 0; i < tree.VisibleColumnCount; i++) {
				columns[i] = tree.GetColumnByVisibleIndex(i);
			}
			foreach(TreeListNode node in tree.Selection) {
				nodesText += GetNodeText(node, columns) + LineDelimiter;
			}
			if(nodesText.EndsWith(LineDelimiter))
				nodesText = nodesText.Remove(nodesText.Length - LineDelimiter.Length, LineDelimiter.Length);
            
			Clipboard.SetDataObject(nodesText);
		}

		public static void SelectAll(TreeList tree) {
			TreeListOperationAccumulateNode op = new TreeListOperationAccumulateNode();
			tree.NodesIterator.DoOperation(op);
			tree.BeginUpdate(); 
			try {
				tree.Selection.Clear();
				tree.Selection.Add(op.Nodes);
			}
			finally {
				tree.EndUpdate(); 
			}
		}
	}

	public class TreeListOperationAccumulateNode : TreeListVisibleNodeOperation {
		ArrayList al = new ArrayList();
		public override void Execute(TreeListNode node) {
			al.Add(node);
		}
		public ArrayList Nodes { get { return al; } }
	}
}
