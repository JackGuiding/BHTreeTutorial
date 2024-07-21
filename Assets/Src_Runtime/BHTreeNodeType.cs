namespace BHTreeTutorial {

    public enum BHTreeNodeType {
        Action, // 行为节点
        Sequence, // 顺序节点
        Selector, // 选择节点
        ParallelAnd, // 并行与节点
        ParallelOr, // 并行或节点
    }
}