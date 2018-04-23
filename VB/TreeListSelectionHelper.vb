Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Windows.Forms
Imports DevExpress.XtraTreeList
Imports DevExpress.XtraTreeList.Nodes
Imports DevExpress.XtraTreeList.Columns
Imports DevExpress.XtraTreeList.Nodes.Operations

Namespace SelectAllAndCopy
	Public Class TreeListSelectionHelper
		Public Shared Function GetNodeText(ByVal node As TreeListNode, ByVal columns() As TreeListColumn) As String
			Const Delimiter As String = Constants.vbTab
			Dim result As String = String.Empty
			If node Is Nothing Then
				Return result
			End If
			For Each column As TreeListColumn In columns
				result &= node.GetDisplayText(column) & Delimiter
			Next column
			If result.EndsWith(Delimiter) Then
				result = result.Remove(result.Length - Delimiter.Length, Delimiter.Length)
			End If
			Return result
		End Function
		Public Shared Function GetNodeText(ByVal node As TreeListNode) As String
			If node Is Nothing Then
				Return String.Empty
			End If
			Dim tree As TreeList = node.TreeList
			Dim columns(tree.VisibleColumnCount - 1) As TreeListColumn
			For i As Integer = 0 To tree.VisibleColumnCount - 1
				columns(i) = tree.GetColumnByVisibleIndex(i)
			Next i
			Return GetNodeText(node, columns)
		End Function
		Public Shared Sub CopySelectedToClipboard(ByVal tree As TreeList)
			If tree Is Nothing OrElse tree.Selection.Count = 0 Then
				Return
			End If
			Const LineDelimiter As String = Constants.vbLf
			Dim nodesText As String = String.Empty
			Dim columns(tree.VisibleColumnCount - 1) As TreeListColumn
			For i As Integer = 0 To tree.VisibleColumnCount - 1
				columns(i) = tree.GetColumnByVisibleIndex(i)
			Next i
			For Each node As TreeListNode In tree.Selection
				nodesText &= GetNodeText(node, columns) & LineDelimiter
			Next node
			If nodesText.EndsWith(LineDelimiter) Then
				nodesText = nodesText.Remove(nodesText.Length - LineDelimiter.Length, LineDelimiter.Length)
			End If

			Clipboard.SetDataObject(nodesText)
		End Sub

		Public Shared Sub SelectAll(ByVal tree As TreeList)
			Dim op As New TreeListOperationAccumulateNode()
			tree.NodesIterator.DoOperation(op)
			tree.BeginUpdate()
			Try
				tree.Selection.Clear()
				tree.Selection.Add(op.Nodes)
			Finally
				tree.EndUpdate()
			End Try
		End Sub
	End Class

	Public Class TreeListOperationAccumulateNode
		Inherits TreeListVisibleNodeOperation
		Private al As New ArrayList()
		Public Overrides Sub Execute(ByVal node As TreeListNode)
			al.Add(node)
		End Sub
		Public ReadOnly Property Nodes() As ArrayList
			Get
				Return al
			End Get
		End Property
	End Class
End Namespace
