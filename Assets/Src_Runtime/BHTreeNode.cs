using System;
using System.Collections.Generic;

namespace BHTreeTutorial {

    // 1. 容器节点
    // 2. 行为节点
    public class BHTreeNode {

        // 表达节点类型
        public BHTreeNodeType type;
        public BHTreeNodeStatus status;
        public Func<bool> PreconditionHandle;

        // 子节点
        public List<BHTreeNode> children;
        public BHTreeNode activeChild;

        public Func<float, BHTreeNodeStatus> ActEnterHandle;
        public Func<float, BHTreeNodeStatus> ActRunningHandle;

        public BHTreeNode() { }

        public void InitAsAction() {
            // type = BHTreeNodeType.Action;
        }

        public void Reset() {
            status = BHTreeNodeStatus.NotEntered;
            if (type != BHTreeNodeType.Action) {
                foreach (BHTreeNode child in children) {
                    child.Reset();
                }
            }
        }

        // ==== 容器 ====
        #region Container
        public void InitAsContainer(BHTreeNodeType type) {
            if (type == BHTreeNodeType.Action) {
                throw new Exception("Action 节点不能作为容器节点");
            }
            this.type = type;
            children = new List<BHTreeNode>();
        }

        public void Container_AddChild(BHTreeNode child) {
            if (type == BHTreeNodeType.Action) {
                throw new Exception("Action 节点无法添加子节点");
            }
            children.Add(child);
        }

        public BHTreeNodeStatus Execute(float dt) {
            // 容器: 顺序节点 / 选择节点 / 并行与节点 / 并行或节点
            if (type == BHTreeNodeType.Sequence) {
                return Container_Sequence_Execute(dt);
            } else if (type == BHTreeNodeType.Selector) {
                return Container_Selector_Execute(dt);
            } else if (type == BHTreeNodeType.ParallelAnd) {
                return Container_Parallel_And_Execute(dt);
            } else if (type == BHTreeNodeType.ParallelOr) {
                return Container_Parallel_Or_Execute(dt);
            } else if (type == BHTreeNodeType.Action) {
                // 行为节点
                return Action_Execute(dt);
            } else {
                throw new Exception("未知的节点类型");
            }
        }

        BHTreeNodeStatus Container_Sequence_Execute(float dt) {
            if (status == BHTreeNodeStatus.NotEntered) {
                // 能否进入 PreCondition
                if (PreconditionHandle == null || !PreconditionHandle.Invoke()) {
                    status = BHTreeNodeStatus.Done;
                } else {
                    status = BHTreeNodeStatus.Running;
                }
            } else if (status == BHTreeNodeStatus.Running) {
                // 执行子节点
                // 顺序容器:
                // 1. 从左到右执行子节点(第一个发现不为 Done 的子节点)
                // 2. 如果所有子节点都返回 Done, 则返回 Done
                // 同时执行一个
                int doneCount = 0;
                for (int i = 0; i < children.Count; i += 1) {
                    BHTreeNode child = children[i];
                    if (child.status == BHTreeNodeStatus.NotEntered) {
                        _ = child.Execute(dt);
                        break;
                    } else if (child.status == BHTreeNodeStatus.Running) {
                        _ = child.Execute(dt);
                        break;
                    } else {
                        doneCount += 1;
                    }
                }

                if (doneCount >= children.Count) {
                    status = BHTreeNodeStatus.Done;
                }
            } else {
                // Do Nothing
            }
            return status;
        }

        BHTreeNodeStatus Container_Selector_Execute(float dt) {
            if (status == BHTreeNodeStatus.NotEntered) {
                if (PreconditionHandle == null || !PreconditionHandle.Invoke()) {
                    status = BHTreeNodeStatus.Done;
                } else {
                    status = BHTreeNodeStatus.Running;
                }
            } else if (status == BHTreeNodeStatus.Running) {
                // 选择容器:
                // 1. 从左到右执行子节点(第一个发现不为 Done 的子节点, 并记录为 activeChild)
                // 2. activeChild 节点返回 Done, 则返回 Done
                // 3. 如果所有子节点都返回 Done, 则返回 Done
                if (activeChild != null) {
                    BHTreeNodeStatus childStatus = activeChild.Execute(dt);
                    if (childStatus == BHTreeNodeStatus.Done) {
                        status = BHTreeNodeStatus.Done;
                    }
                } else {
                    // 找到一个执行中的节点
                    // 同时执行一个
                    for (int i = 0; i < children.Count; i += 1) {
                        BHTreeNode child = children[i];
                        BHTreeNodeStatus childStatus = child.Execute(dt);
                        if (childStatus != BHTreeNodeStatus.Done) {
                            activeChild = child;
                            break;
                        }
                    }
                    if (activeChild == null) {
                        status = BHTreeNodeStatus.Done;
                    }
                }
            } else {
                // Do Nothing
            }
            return status;
        }

        BHTreeNodeStatus Container_Parallel_And_Execute(float dt) {
            if (status == BHTreeNodeStatus.NotEntered) {
                if (PreconditionHandle == null || !PreconditionHandle.Invoke()) {
                    status = BHTreeNodeStatus.Done;
                } else {
                    status = BHTreeNodeStatus.Running;
                }
            } else if (status == BHTreeNodeStatus.Running) {
                int doneCount = 0;
                // 同时执行所有
                for (int i = 0; i < children.Count; i += 1) {
                    BHTreeNode child = children[i];
                    BHTreeNodeStatus childStatus = child.Execute(dt);
                    if (childStatus == BHTreeNodeStatus.Done) {
                        doneCount += 1;
                    }
                }

                if (doneCount >= children.Count) {
                    status = BHTreeNodeStatus.Done;
                }
            } else {
                // Do Nothing
            }
            return status;
        }

        BHTreeNodeStatus Container_Parallel_Or_Execute(float dt) {
            if (status == BHTreeNodeStatus.NotEntered) {
                if (PreconditionHandle == null || !PreconditionHandle.Invoke()) {
                    status = BHTreeNodeStatus.Done;
                } else {
                    status = BHTreeNodeStatus.Running;
                }
            } else if (status == BHTreeNodeStatus.Running) {
                bool hasDone = false;
                // 同时执行所有
                for (int i = 0; i < children.Count; i += 1) {
                    BHTreeNode child = children[i];
                    BHTreeNodeStatus childStatus = child.Execute(dt);
                    if (childStatus == BHTreeNodeStatus.Done) {
                        hasDone = true;
                    }
                }

                if (hasDone) {
                    status = BHTreeNodeStatus.Done;
                }
            } else {
                // Do Nothing
            }
            return status;
        }
        #endregion

        // ==== 行为 ====
        #region Action
        BHTreeNodeStatus Action_Execute(float dt) {
            if (status == BHTreeNodeStatus.NotEntered) {
                if (PreconditionHandle == null || !PreconditionHandle.Invoke()) {
                    status = BHTreeNodeStatus.Done;
                } else {
                    status = BHTreeNodeStatus.Running;
                    status = ActEnterHandle.Invoke(dt);
                }
            } else if (status == BHTreeNodeStatus.Running) {
                // 执行行为
                status = ActRunningHandle.Invoke(dt);
            } else {
                // Do Nothing
            }
            return status;
        }
        #endregion

    }

}