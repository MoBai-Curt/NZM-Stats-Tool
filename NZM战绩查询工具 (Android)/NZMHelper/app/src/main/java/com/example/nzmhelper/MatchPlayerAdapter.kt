package com.example.nzmhelper

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.nzmhelper.databinding.ItemMatchPlayerBinding
import java.net.URLDecoder

class MatchPlayerAdapter(private val players: List<Pair<PlayerDetail, Boolean>>) : RecyclerView.Adapter<MatchPlayerAdapter.ViewHolder>() {

    class ViewHolder(val binding: ItemMatchPlayerBinding) : RecyclerView.ViewHolder(binding.root)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val binding = ItemMatchPlayerBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return ViewHolder(binding)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val (p, isSelf) = players[position]
        val context = holder.itemView.context

        val rawName = p.nickname ?: "未知"
        holder.binding.tvPlayerName.text = try { URLDecoder.decode(rawName, "UTF-8") } catch (e: Exception) { rawName }
        holder.binding.tvSelfTag.visibility = if (isSelf) View.VISIBLE else View.GONE

        val avatarRaw = p.avatar ?: ""
        val avatarUrl = try {
            if (avatarRaw.isNotEmpty()) URLDecoder.decode(avatarRaw, "UTF-8") else ""
        } catch (e: Exception) { avatarRaw }

        Glide.with(context).load(avatarUrl).circleCrop().into(holder.binding.imgAvatar)

        holder.binding.tvPlayerScore.text = "得分: ${p.base?.score ?: "0"}"

        holder.binding.tvBossDmg.text = p.hunt?.bossDmgCap ?: p.hunt?.bossDmg ?: "0"
        holder.binding.tvMobDmg.text = p.hunt?.mobDmgCap ?: p.hunt?.mobDmg ?: "0"
        holder.binding.tvCoin.text = p.hunt?.coinCap ?: p.hunt?.coin ?: "0"
        holder.binding.tvDeath.text = p.base?.deaths ?: "0"

        val prog = p.hunt?.itemProgressCap ?: p.hunt?.itemProgress
        if (prog != null && prog.required > 0) {
            holder.binding.layoutShard.visibility = View.VISIBLE
            holder.binding.pbShard.max = prog.required
            holder.binding.pbShard.progress = prog.current
            holder.binding.tvShardValue.text = "${prog.current} / ${prog.required}"
            holder.binding.tvShardName.text = prog.itemName ?: "碎片进度"
        } else {
            holder.binding.layoutShard.visibility = View.GONE
        }
    }

    override fun getItemCount() = players.size
}