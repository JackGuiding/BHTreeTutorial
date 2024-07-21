namespace BHTreeTutorial {

    public class BHTree {

        BHTreeNode root;
        public bool isPause;

        public BHTree() { }

        public void InitRoot(BHTreeNode root) {
            this.root = root;
        }

        public void Pause() {
            isPause = true;
        }

        public void Resume() {
            isPause = false;
        }

        public void Execute(float dt) {
            if (isPause) {
                return;
            }
            BHTreeNodeStatus rootStatus = root.Execute(dt);
            if (rootStatus == BHTreeNodeStatus.Done) {
                root.Reset();
            }
        }

    }
}