package com.example.nzmhelper

import android.graphics.Color
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.bumptech.glide.load.resource.drawable.DrawableTransitionOptions
import com.example.nzmhelper.databinding.ItemFragmentBinding

class FragmentAdapter(private val items: List<CollectionItem>) : RecyclerView.Adapter<FragmentAdapter.ViewHolder>() {

    class ViewHolder(val binding: ItemFragmentBinding) : RecyclerView.ViewHolder(binding.root)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ItemFragmentBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = items[position]
        val context = holder.itemView.context
        val prog = item.itemProgress

        holder.binding.tvName.text = item.weaponName ?: item.itemName ?: "未知道具"

        val quality = item.quality ?: 1
        val qualityColor = when (quality) {
            4 -> "#f59e0b" // 传说：橙色
            3 -> "#a855f7" // 史诗：紫色
            2 -> "#3b82f6" // 稀有：蓝色
            else -> "#9ca3af" // 普通：灰色
        }

        holder.binding.viewQualityBar.setBackgroundColor(Color.parseColor(qualityColor))

        Glide.with(context)
            .load(item.pic ?: item.icon)
            .transition(DrawableTransitionOptions.withCrossFade())
            .placeholder(R.drawable.bg_card)
            .into(holder.binding.imgIcon)

        if (prog != null) {
            val current = prog.current
            val required = if (prog.required > 0) prog.required else 100

            holder.binding.pbFragment.max = required
            holder.binding.pbFragment.progress = current

            holder.binding.tvProgress.text = "$current / $required"

            val percent = (current * 100 / required)
            holder.binding.tvPercent.text = "$percent%"

            val pbDrawable = holder.binding.pbFragment.progressDrawable
            if (percent >= 100) {
                holder.binding.tvPercent.setTextColor(Color.parseColor("#10b981")) // 满进度绿色
            } else {
                holder.binding.tvPercent.setTextColor(Color.parseColor("#3b82f6"))
            }
        } else {
            holder.binding.tvProgress.text = "0 / 0"
            holder.binding.tvPercent.text = "0%"
            holder.binding.pbFragment.progress = 0
        }
    }

    override fun getItemCount() = items.size
}