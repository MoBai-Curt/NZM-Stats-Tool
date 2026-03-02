package com.example.nzmhelper

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.nzmhelper.databinding.ItemCollectionBinding
import java.net.URLDecoder

class CollectionAdapter(private val items: List<CollectionItem>) : RecyclerView.Adapter<CollectionAdapter.ViewHolder>() {

    class ViewHolder(val binding: ItemCollectionBinding) : RecyclerView.ViewHolder(binding.root)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ItemCollectionBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = items[position]
        val context = holder.itemView.context

        val rawName = item.weaponName ?: item.trapName ?: item.itemName ?: "未知"
        holder.binding.tvName.text = try { URLDecoder.decode(rawName, "UTF-8") } catch (e: Exception) { rawName }

        val url = item.pic ?: item.icon
        if (!url.isNullOrEmpty()) Glide.with(context).load(url).into(holder.binding.imgIcon)

        val q = (item.quality ?: 1).let { if(it > 0) it - 1 else 0 }.let { if(it < 4) it else 3 }
        val colors = intArrayOf(R.color.quality_common, R.color.quality_rare, R.color.quality_epic, R.color.quality_legend)
        val names = arrayOf("普通", "稀有", "史诗", "传说")

        holder.binding.tvQuality.setTextColor(ContextCompat.getColor(context, colors[q]))
        holder.binding.tvQuality.text = names[q]
        holder.itemView.alpha = if (item.owned == true) 1.0f else 0.4f
    }

    override fun getItemCount() = items.size
}