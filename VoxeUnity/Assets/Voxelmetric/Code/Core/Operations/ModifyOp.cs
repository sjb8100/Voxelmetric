﻿using Voxelmetric.Code.Data_types;

namespace Voxelmetric.Code.Core.Operations
{
    public abstract class ModifyOp
    {
        //! Parent action
        public readonly ModifyBlockContext parentContext;
        //! Block which is to be worked with
        protected readonly BlockData blockData;
        //! If true we want to mark the block as modified
        protected readonly bool setBlockModified;

        protected ModifyOp(BlockData blockData, bool setBlockModified, ModifyBlockContext parentContext = null)
        {
            this.parentContext = parentContext;
            this.blockData = blockData;
            this.setBlockModified = setBlockModified;

            if (parentContext!=null)
                parentContext.RegisterChildAction();
        }

        public void Apply(ChunkBlocks blocks)
        {
            OnSetBlocks(blocks);
            OnPostSetBlocks(blocks);
        }

        protected abstract void OnSetBlocks(ChunkBlocks blocks);
        protected abstract void OnPostSetBlocks(ChunkBlocks blocks);
        protected abstract bool IsRanged();
    }
}
