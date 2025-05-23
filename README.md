
# 学龄期ADHD儿童 评估程序

该程序设计用来给学龄期ADHD儿童进行相关评估。

一、摘要
建议框架：
本研究基于Unity开发的微信小程序，整合Stroop、Flanker、Memory等认知任务，通过动态游戏化评估学龄期ADHD儿童的执行功能缺陷。通过量化反应时、正确率等行为指标，结合前庭平衡与热/冷执行功能理论，构建多维评估模型。研究旨在验证游戏化工具在ADHD筛查中的效度，为临床诊断提供客观量化依据。

二、绪论
研究背景
注意缺陷多动障碍（ADHD）是一种神经发育性疾病，约50%-60%患者症状持续至成年期，常共患对立违抗障碍（ODD）、焦虑障碍及抽动症，其自杀风险与犯罪率显著高于普通人群（Barkley, 2015）。核心症状表现为：
1.	注意障碍
患者在课堂听讲、作业完成等持续性任务中难以维持注意力，易受外界干扰，表现为频繁活动转换、细节遗漏及信息吸收障碍。值得注意的是，约30% ADHD儿童共患发展性语言障碍（DLD），导致语言表达与社交互动困难（Hvolby, 2023）。
2.	多动与冲动
行为特征包括过度躯体活动（如无目的跑动）、冲动决策（如未计划行动）及情绪调节缺陷（如延迟满足困难）。神经影像学研究提示，此类症状与前额叶-纹状体环路多巴胺能低下密切相关（Volkow et al., 2021）。
3.	神经生化机制
多巴胺（DA）、去甲肾上腺素（NE）系统功能低下与5-羟色胺（5-HT）功能亢进构成核心病理基础。DA/NE不足导致前额叶抑制控制减弱，而5-HT异常升高可能加剧情绪波动（Tripp & Wickens, 2008）。
4.	前庭功能与执行缺陷
65% ADHD儿童存在前庭功能减退（Edith Wolfson医学中心, 2022），导致平衡能力下降与小脑-前额叶连接异常，进一步损害计划能力与抑制控制（如汉诺塔任务完成率降低15%）。

研究意义
本研究通过开发集成Stroop、Flanker等经典任务的游戏化评估工具，实现以下突破：
1.	动态量化评估：突破传统量表主观性局限，通过反应时、正确率等客观指标精准捕捉执行功能缺陷。
2.	前庭-认知联动分析：结合平衡功能测试（如CAMI运动模仿），揭示ADHD儿童感觉统合与执行功能的交互机制。
3.	热/冷执行功能分层：含奖赏机制的“热执行任务”（如积分激励）预测外化行为风险，而“冷执行任务”（如Stroop）评估内化问题，指导个性化干预设计。

研究方法
1.	技术框架
基于Unity引擎开发微信小程序，包含6大核心模块：
o	Stroop：色词冲突抑制（如“红”字显示为绿色）
o	Flanker：方向干扰抑制（如中心箭头与周边箭头方向冲突）
o	Memory Car：空间工作记忆（如汽车位置记忆）
o	Memory animal：空间工作记忆（如动物位置记忆）
o	Switch：任务切换灵活性（如数字奇偶-大小转换）
o	Rotation：心理旋转能力（如镜像图形匹配）

2.	实验设计
o	练习环节（Prac-proc）：固定时长（2000ms）与轮次（10轮），达标阈值60%-70%，确保受试者理解任务规则。
o	测试环节（Test-proc）：缩短刺激呈现时间（1000ms）、增加轮次（15-20轮），通过动态难度提升数据区分度。
3.	数据分析
采集正确率、反应时、任务切换代价等指标，采用机器学习算法（如随机森林）构建ADHD风险预测模型，并与Conners量表评分进行效度验证。


## License

[MIT](https://choosealicense.com/licenses/mit/)


## Screenshots

![App Screenshot](https://github.com/lijingjie5/PE/blob/main/图片1.jpg)
![App Screenshot](https://github.com/lijingjie5/PE/blob/main/图片3.jpg)
![App Screenshot](https://github.com/lijingjie5/PE/blob/main/图片13.jpg)


## 参考文献

1.	注意缺陷多动障碍综述（谢松林，2024）：系统梳理ADHD的神经机制、临床表现与治疗策略，强调多模态评估的重要性。
2.	Child executive function and future externalizing and internalizing problems（Yang et al., 2022）：前瞻性元分析揭示执行功能缺陷与行为问题的长期关联，为游戏化干预提供理论依据。
3.	A novel digital intervention for actively reducing severity of paediatric ADHD（The Lancet Digital Health, 2020）。
4.	Child executive function and future externalizing and internalizing problems: A meta-analysis（Clinical Psychology Review, 2022）。
5.	Real-world goal-directed behavior reveals aberrant functional brain connectivity in children with ADHD（PLOS ONE, 2025）。
6.	基于注意力驱动的ADHD家庭干预研究（Neuro, 2019）。
7.	Real-world goal-directed behavior reveals aberrant functional brain connectivity in children with ADHD（PLOS ONE, 2025）。
8.	Computerized Assessment of Motor Imitation (CAMI) for ASD and ADHD differentiation（英国精神病学杂志, 2025）。
9.	A novel digital intervention for actively reducing severity of paediatric ADHD（The Lancet Digital Health, 2020）。
