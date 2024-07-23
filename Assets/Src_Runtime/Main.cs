using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BHTreeTutorial {

    public class Main : MonoBehaviour {

        bool isHumanWDown;
        bool isAIWDown;

        BHTree tree;

        public bool hasTarget;
        public int hp;
        public int mp;
        public float skill1CD;
        public float skill2CD;

        void Start() {

            hasTarget = false;
            hp = 100;
            mp = 10;
            skill1CD = 5;
            skill2CD = 1;

            // ========================

            // root: Sequence
            // - search: ParallelOr
            //     - searchHp: Action
            //     - searchMp: Action
            // - attack: Selector
            //     - skill1: Action
            //     - skill2: Action

            // RoleDomain.Spawn(GameContext ctx) {}

            // Search: ParallelOr
            BHTreeNode searchHpAction = new BHTreeNode("Search HP");
            searchHpAction.ActRunningHandle = (dt) => {
                if (hp < 50) {
                    hasTarget = true;
                    Debug.Log($"找到目标: hp < 50");
                }
                return BHTreeNodeStatus.Done;
            };
            searchHpAction.InitAsAction();

            BHTreeNode searchMpAction = new BHTreeNode("Search MP");
            searchMpAction.ActRunningHandle = (dt) => {
                if (mp < 5) {
                    hasTarget = true;
                    Debug.Log($"找到目标: mp < 5");
                }
                return BHTreeNodeStatus.Done;
            };
            searchMpAction.InitAsAction();

            BHTreeNode searchContainer = new BHTreeNode("Search Container");
            searchContainer.PreconditionHandle = () => {
                return !hasTarget;
            };
            searchContainer.InitAsContainer(BHTreeNodeType.ParallelOr);
            searchContainer.Container_AddChild(searchHpAction);
            searchContainer.Container_AddChild(searchMpAction);

            // Attack: Selector
            BHTreeNode skill1Action = new BHTreeNode("Skill1");
            skill1Action.PreconditionHandle = () => {
                bool allowEnter = hasTarget;
                allowEnter &= (skill1CD <= 0);
                return allowEnter;
            };
            skill1Action.ActRunningHandle = (dt) => {
                Debug.Log("技能1");
                skill1CD = 5;
                return BHTreeNodeStatus.Done;
            };
            skill1Action.InitAsAction();

            BHTreeNode skill2Action = new BHTreeNode("Skill2");
            skill2Action.PreconditionHandle = () => {
                bool allowEnter = hasTarget;
                allowEnter &= (skill2CD <= 0);
                return allowEnter;
            };
            skill2Action.ActRunningHandle = (dt) => {
                Debug.Log("技能2");
                skill2CD = 1;
                return BHTreeNodeStatus.Done;
            };
            skill2Action.InitAsAction();

            BHTreeNode attackContainer = new BHTreeNode("Attack Container");
            attackContainer.PreconditionHandle = () => {
                return hasTarget;
            };
            attackContainer.InitAsContainer(BHTreeNodeType.SelectorSequence);
            attackContainer.Container_AddChild(skill1Action);
            attackContainer.Container_AddChild(skill2Action);

            // Root
            BHTreeNode root = new BHTreeNode("Root");
            root.InitAsContainer(BHTreeNodeType.Sequence);
            root.Container_AddChild(searchContainer);
            root.Container_AddChild(attackContainer);

            tree = new BHTree();
            tree.InitRoot(root);

        }

        void Update() {

            float dt = Time.deltaTime;
            tree.Execute(dt);

            if (Input.GetKeyDown(KeyCode.A)) {
                hp -= 20;
            } else if (Input.GetKeyDown(KeyCode.D)) {
                mp -= 5;
            }

            skill1CD -= dt;
            skill2CD -= dt;

            // Process Input
            // 人类的输入
            // if (Input.GetKey(KeyCode.W)) {
            //     isHumanWDown = true;
            // } else {
            //     isHumanWDown = false;
            // }

            // if (Time.time % 2 == 0) {
            //     isAIWDown = true;
            // } else {
            //     isAIWDown = false;
            // }

            // // DoLogic
            // if (isHumanWDown) {
            //     Debug.Log("人类前进");
            // }

            // if (isAIWDown) {
            //     Debug.Log("AI前进");
            // }

        }

    }
}
